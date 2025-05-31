import sys
import json
import re
from ruamel.yaml import YAML

yaml_parser = YAML(typ='rt')
yaml_parser.allow_duplicate_keys = True

def split_blocks(content):
    pattern = r"^--- !u!(\d+) &(\d+)\s*$"
    blocks = []
    current = None

    for line in content.splitlines():
        match = re.match(pattern, line)
        if match:
            if current:
                blocks.append(current)
            current = {
                "type": match.group(1),
                "id": match.group(2),
                "lines": []
            }
        elif current:
            current["lines"].append(line)

    if current:
        blocks.append(current)

    return blocks

def parse_yaml_blocks(blocks):
    parsed = []
    idmap = {}

    for block in blocks:
        yaml_text = "\n".join(block["lines"])
        try:
            docs = list(yaml_parser.load_all(yaml_text))
            docs_cleaned = [doc for doc in docs if doc is not None]
            if not docs_cleaned:
                continue

            if len(docs_cleaned) == 1:
                data = docs_cleaned[0]
            else:
                data = {
                    "_multi_doc": True,
                    "documents": docs_cleaned
                }

            if block["type"] == "1":
                name = data.get("GameObject", {}).get("m_Name")
                if name:
                    idmap[block["id"]] = name

        except Exception as e:
            data = {
                "_parse_error": True,
                "_error_message": str(e),
                "_raw": yaml_text
            }

        parsed.append({
            "type": block["type"],
            "id": block["id"],
            "data": data
        })

    return parsed, idmap

def convert_from_string(content):
    blocks = split_blocks(content)

    # === 블록이 없을 경우 전체 YAML 문서로 처리 ===
    if not blocks:
        try:
            data = yaml_parser.load(content)
            return [data] if data is not None else []
        except Exception as e:
            return [{
                "_parse_error": True,
                "_error_message": str(e),
                "_raw": content
            }]

    # === 기존 블록 기반 처리 ===
    parsed_data, idmap = parse_yaml_blocks(blocks)
    result = parsed_data
    if idmap:
        result.append({"_idmap": idmap})
    return result

if __name__ == "__main__":
    try:
        # 입력 소스 선택
        if len(sys.argv) == 2 and sys.argv[1] != "--from-stdin":
            input_path = sys.argv[1]
            with open(input_path, "r", encoding="utf-8") as f:
                content = f.read()
        else:
            content = sys.stdin.read()

        result = convert_from_string(content)
        print(json.dumps(result, indent=2, ensure_ascii=False))

    except Exception as e:
        print(json.dumps({"_error": str(e)}), file=sys.stderr)
        sys.exit(1)
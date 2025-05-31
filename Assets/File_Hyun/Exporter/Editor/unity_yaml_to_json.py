import sys
import json
import re
import os
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

def convert(filepath):
    with open(filepath, "r", encoding="utf-8") as f:
        content = f.read()

    blocks = split_blocks(content)
    parsed_data, idmap = parse_yaml_blocks(blocks)

    result = parsed_data
    if idmap:
        result.append({"_idmap": idmap})
    return result

def save_result_as_json(output_path, result):
    with open(output_path, "w", encoding="utf-8") as f:
        json.dump(result, f, indent=2, ensure_ascii=False)
    print(f"[OK] 저장 완료: {output_path}")

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python unity_yaml_to_json_v3_clean.py <input_yaml> [output_json]")
        sys.exit(1)

    input_path = sys.argv[1]
    output_path = sys.argv[2] if len(sys.argv) > 2 else input_path + ".json"

    try:
        result = convert(input_path)
        save_result_as_json(output_path, result)
    except Exception as e:
        print(f"[ERROR] 오류 발생: {e}")
import sys
import json
import re
import os
from ruamel.yaml import YAML

yaml_parser = YAML(typ='rt')
yaml_parser.allow_duplicate_keys = True

TYPE_MAP = {
    "1": "GameObject",
    "2": "Component",
    "3": "LevelGameManager",
    "4": "Transform",
    "5": "TimeManager",
    "6": "AudioManager",
    "8": "InputManager",
    "9": "GraphicsSettings",
    "11": "PlayerSettings",
    "13": "QualitySettings",
    "15": "TagManager",
    "18": "NavMeshAreas",
    "19": "PhysicsManager",
    "20": "Camera",
    "21": "Material",
    "23": "MeshRenderer",
    "25": "Texture",
    "26": "Texture2D",
    "28": "OcclusionCullingSettings",
    "30": "Mesh",
    "33": "MeshFilter",
    "43": "OcclusionPortal",
    "45": "LODGroup",
    "48": "Shader",
    "49": "TextAsset",
    "50": "Rigidbody2D",
    "54": "CircleCollider2D",
    "55": "BoxCollider2D",
    "56": "CompositeCollider2D",
    "57": "EdgeCollider2D",
    "58": "PolygonCollider2D",
    "59": "TilemapCollider2D",
    "60": "FixedJoint2D",
    "61": "RelativeJoint2D",
    "62": "SpringJoint2D",
    "63": "TargetJoint2D",
    "64": "HingeJoint2D",
    "65": "SliderJoint2D",
    "66": "WheelJoint2D",
    "68": "NavMeshData",
    "74": "AudioImporter",
    "81": "AudioListener",
    "82": "AudioSource",
    "83": "AudioClip",
    "89": "LightingSettings",
    "92": "Sprite",
    "95": "Animator",
    "96": "SpeedTreeWindAsset",
    "102": "CanvasGroup",
    "104": "BillboardAsset",
    "108": "Light",
    "109": "LightmapSettings",
    "115": "SpriteRenderer",
    "117": "Canvas",
    "118": "CanvasRenderer",
    "120": "Text",
    "124": "MeshCollider",
    "135": "TerrainCollider",
    "136": "WheelCollider",
    "137": "CapsuleCollider",
    "138": "SphereCollider",
    "146": "AudioReverbZone",
    "154": "UnityConnectSettings",
    "156": "VideoPlayer",
    "157": "VideoClip",
    "195": "AssemblyDefinitionAsset",
    "197": "AssemblyDefinitionImporter",
    "198": "PlatformModuleSetup",
    "199": "PackageManifest",
    "200": "AnimatorOverrideController",
    "212": "SpriteRenderer",
    "213": "SpriteAtlas",
    "215": "UnityAnalyticsSettings",
    "218": "SignalAsset",
    "222": "Text",
    "223": "Canvas",
    "224": "RectTransform",
    "225": "CanvasRenderer",
    "226": "SpriteMask",
    "233": "TMP_Text",
    "234": "TMP_FontAsset",
    "235": "TMP_SpriteAsset",
    "236": "TMP_InputField",
    "237": "TMP_TextMeshProUGUI",
    "238": "TMP_TextMeshPro",
    "240": "Grid",
    "241": "Tilemap",
    "242": "Tile",
    "243": "TilemapRenderer",
    "248": "LocalizationSettings",
    "256": "AssetBundleManifest",
    "102900": "MonoScript",
    "114": "MonoBehaviour"
}

REMOVE_KEYS = {
    "m_ObjectHideFlags", "m_PrefabInstance", "m_PrefabAsset",
    "m_CorrespondingSourceObject", "m_EditorHideFlags",
    "m_Name", "m_EditorClassIdentifier", "m_Icon"
}

# Optional script GUID → path map (to be filled by external loader)
script_guid_map = {}

def load_script_guid_map(path):
    global script_guid_map
    if os.path.exists(path):
        with open(path, 'r', encoding='utf-8') as f:
            script_guid_map = json.load(f)

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
    name_map = {}  # fileID -> object name
    raw_map = {}   # fileID -> raw data
    transform_parents = {}  # fileID -> parent fileID
    transform_children = {}  # fileID -> list[fileID]

    for block in blocks:
        yaml_text = "\n".join(block["lines"])
        try:
            docs = list(yaml_parser.load_all(yaml_text))
            docs_cleaned = [doc for doc in docs if doc is not None]
            if not docs_cleaned:
                continue

            data = docs_cleaned[0] if len(docs_cleaned) == 1 else {"_multi_doc": True, "documents": docs_cleaned}
            typename = TYPE_MAP.get(block["type"], f"UnknownType_{block['type']}")

            raw_map[block["id"]] = {
                "type": typename,
                "id": block["id"],
                "data": data
            }

            if typename == "GameObject":
                name = data.get("GameObject", {}).get("m_Name")
                if name:
                    name_map[block["id"]] = name

            elif typename == "Transform":
                go = data.get("Transform", {}).get("m_GameObject", {}).get("fileID")
                if go:
                    name = name_map.get(str(go))
                    if name:
                        name_map[block["id"]] = name + ".Transform"
                parent = data.get("Transform", {}).get("m_Father", {}).get("fileID")
                if parent:
                    transform_parents[str(block["id"])] = str(parent)
                    transform_children.setdefault(str(parent), []).append(block["id"])

        except Exception as e:
            raw_map[block["id"]] = {
                "type": "ParseError",
                "id": block["id"],
                "data": {
                    "_parse_error": True,
                    "_error_message": str(e),
                    "_raw": yaml_text
                }
            }

    return raw_map, name_map, transform_parents, transform_children

def enrich_references(obj, name_map):
    if isinstance(obj, dict):
        new_obj = {}
        for k, v in obj.items():
            new_obj[k] = enrich_references(v, name_map)
            if k == "fileID" and isinstance(v, int | str):
                ref = name_map.get(str(v))
                if ref:
                    new_obj["refName"] = ref
        return new_obj
    elif isinstance(obj, list):
        return [enrich_references(i, name_map) for i in obj]
    else:
        return obj

def clean_data(data, type_hint=None):
    if isinstance(data, dict):
        return {
            k: clean_data(v, type_hint)
            for k, v in data.items()
            if v not in (None, 0, {}, [], "") and (k not in REMOVE_KEYS or type_hint == "MonoBehaviour")
        }
    elif isinstance(data, list):
        return [clean_data(i, type_hint) for i in data if i not in (None, {}, [], 0, "")]
    else:
        return data

def extract_script_name(monobehaviour):
    script = monobehaviour.get("m_Script", {})
    guid = script.get("guid")
    if guid:
        return script_guid_map.get(guid, guid[:8]), guid
    return None, None

def summarize_persistent_calls(data):
    if not isinstance(data, dict):
        return
    for key, val in list(data.items()):
        if isinstance(val, dict):
            if "m_PersistentCalls" in val:
                calls = val["m_PersistentCalls"].get("m_Calls", [])
                summary = []
                for c in calls:
                    method = c.get("m_MethodName")
                    target = c.get("m_Target", {}).get("refName") or str(c.get("m_Target", {}).get("fileID"))
                    if method and target:
                        summary.append(f"{method} → {target}")
                data[f"{key}_summary"] = summary
            else:
                summarize_persistent_calls(val)


def build_gpt_structure(raw_map, name_map, transform_parents, transform_children):
    gameobjects = []
    attached = {k: [] for k in raw_map if raw_map[k]["type"] == "GameObject"}

    for id_, entry in raw_map.items():
        if entry["type"] == "GameObject":
            continue

        data = entry["data"]
        game_id = None

        if "MonoBehaviour" in data:
            game_id = str(data["MonoBehaviour"].get("m_GameObject", {}).get("fileID"))
        elif "Transform" in data:
            game_id = str(data["Transform"].get("m_GameObject", {}).get("fileID"))

        if game_id in attached:
            enriched = enrich_references(data, name_map)
            summarize_persistent_calls(enriched)
            cleaned = clean_data(enriched, type_hint=entry["type"])

            component = {
                "type": entry["type"],
                "data": cleaned
            }

            if entry["type"] == "MonoBehaviour":
                scriptName, guid = extract_script_name(data["MonoBehaviour"])
                if scriptName:
                    component["scriptName"] = scriptName
                if guid:
                    component["scriptGUID"] = guid

            attached[game_id].append(component)

    for id_, entry in raw_map.items():
        if entry["type"] != "GameObject":
            continue

        enriched = enrich_references(entry["data"], name_map)
        cleaned = clean_data(enriched)

        children_ids = transform_children.get(id_, [])
        children_names = [name_map.get(cid, cid) for cid in children_ids]

        go = {
            "name": name_map.get(id_, "Unnamed"),
            "id": id_,
            "components": attached.get(id_, []),
            "children": children_names if children_names else None,
            "raw": cleaned
        }

        gameobjects.append(go)

    return {"GameObjects": gameobjects}

def convert_from_string(content):
    blocks = split_blocks(content)

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

    raw_map, name_map, parents, children = parse_yaml_blocks(blocks)
    structured = build_gpt_structure(raw_map, name_map, parents, children)
    return structured

if __name__ == "__main__":
    try:
        if "--script-map" in sys.argv:
            idx = sys.argv.index("--script-map")
            map_path = sys.argv[idx + 1]
            load_script_guid_map(map_path)

        if len(sys.argv) >= 2 and sys.argv[1] != "--from-stdin" and not sys.argv[1].startswith("--"):
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
import sounddevice as sd
import json

class Direction:
    Input = "Input"
    Output = "Output"

def list_devices_json():
    devices = sd.query_devices()
    device_list = []

    for idx, dev in enumerate(devices):
        #if dev['max_output_channels'] >= channels_required and int(dev['default_samplerate']) == samplerate_required:
            device_list.append({
                "Index": idx,
                "Name": dev["name"],
                "Channels": dev["max_output_channels"],
                "SampleRate": int(dev["default_samplerate"]),
                "direction": Direction.Output
            })

        #if dev['max_input_channels'] >= channels_required and int(dev['default_samplerate']) == samplerate_required:
            device_list.append({
                "Index": idx,
                "Name": dev["name"],
                "Channels": dev["max_input_channels"],
                "SampleRate": int(dev["default_samplerate"]),
                "direction": Direction.Input
            })

    print(json.dumps(device_list, indent=4, ensure_ascii=False))

if __name__ == "__main__":
    list_devices_json()

import sounddevice as sd

input_device_index = 1    # CABLE Output (sesi buradan al)
output_device_index = 10  # Huawei FreeBuds (sesi buraya ver)

samplerate = 44100
channels = 2  # Stereo çünkü ikisi de destekliyor

def callback(indata, outdata, frames, time, status):
    if status:
        print("⚠️", status)
    outdata[:] = indata

try:
    with sd.Stream(device=(input_device_index, output_device_index),
                   samplerate=samplerate,
                   channels=channels,
                   dtype='float32',
                   callback=callback):
        print("🎧 Yayın başladı. Çıkmak için Ctrl+C")
        while True:
            sd.sleep(1000)
except KeyboardInterrupt:
    print("⏹️ Yayın durduruldu.")
except Exception as e:
    print("Hata:", e)

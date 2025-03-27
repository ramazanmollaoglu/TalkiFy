import sounddevice as sd
import whisper
import numpy as np
import queue
import tempfile
import os
import time
import threading
import soundfile as sf
import torch
import win32security
import subprocess
import argparse
import sys
# Cihaz indexleri
INPUT_DEVICE_INDEX = 1  # CABLE Output
OUTPUT_DEVICE_INDEX = 10  # Huawei FreeBuds

# Parametreler
SAMPLERATE = 48000
CHANNELS = 2
WHISPER_SAMPLERATE = 16000
SILENCE_THRESHOLD = 0.01
MIN_AUDIO_LENGTH = 8  # saniye

parser = argparse.ArgumentParser(description="Ses yönlendirme")
parser.add_argument("--input", type=int, default=INPUT_DEVICE_INDEX, help="Input device index")
parser.add_argument("--output", type=int, default=OUTPUT_DEVICE_INDEX, help="Output device index")
parser.add_argument("--samplerate", type=int, default=SAMPLERATE, help="Sample rate (Hz)")
parser.add_argument("--channels", type=int, default=CHANNELS, help="Kanal sayısı")
args = parser.parse_args()
INPUT_DEVICE_INDEX = args.input
OUTPUT_DEVICE_INDEX = args.output
SAMPLERATE = args.samplerate
CHANNELS = args.channels
sys.stdout.reconfigure(encoding='utf-8', line_buffering=True)
# FFmpeg kontrolü
try:
    subprocess.check_output(["ffmpeg", "-version"])
    print("FFmpeg bulundu.")
except FileNotFoundError:
    print("FFmpeg eksik, PATH'e ekleniyor.")
    os.environ["PATH"] += os.pathsep + r"C:\ffmpeg\bin"

# Whisper cihaz ve model yüklemesi
device = "cuda" if torch.cuda.is_available() else "cpu"
print(f"Whisper modeli yüklendi. Kullanılan cihaz: {device}")
model = whisper.load_model("large-v3").to(device)

# Ses işleyici sınıfı
class AudioProcessor:
    def __init__(self):
        self.audio_queue = queue.Queue()
        self.samplerate = WHISPER_SAMPLERATE
        self.min_audio_samples = self.samplerate * MIN_AUDIO_LENGTH

    def is_silent(self, audio_data):
        return np.max(np.abs(audio_data)) < SILENCE_THRESHOLD

    def process_audio(self):
        audio_buffer = np.array([], dtype=np.float32)
        while True:
            try:
                audio_chunk = self.audio_queue.get()
                audio_chunk_resampled = self.resample(audio_chunk, SAMPLERATE, WHISPER_SAMPLERATE)
                audio_buffer = np.append(audio_buffer, audio_chunk_resampled.flatten())

                if len(audio_buffer) >= self.min_audio_samples:
                    if self.is_silent(audio_buffer):
                        print("Sessizlik algılandı.")
                        audio_buffer = np.array([], dtype=np.float32)
                        continue

                    with tempfile.NamedTemporaryFile(suffix=".wav", delete=False) as temp_file:
                        temp_path = temp_file.name
                    sf.write(temp_path, audio_buffer, self.samplerate)

                    sd_sec = win32security.SECURITY_DESCRIPTOR()
                    sd_sec.SetSecurityDescriptorDacl(1, None, 0)
                    win32security.SetFileSecurity(temp_path, win32security.DACL_SECURITY_INFORMATION, sd_sec)

                    try:
                        result = model.transcribe(temp_path, fp16=torch.cuda.is_available())
                        #result_tr = model.transcribe(temp_path, task="translate", fp16=torch.cuda.is_available(), language="tr")
                        text = result["text"].strip()
                        #text_tr = result_tr["text"].strip()
                        print("#eng:", text if text else "Eng Algılanmadı")
                        #print("#tr:", text_tr if text_tr else "Tr Algılanmadı")
                    except Exception as e:
                        print("Transkripsiyon hatası:", e)
                    finally:
                        os.unlink(temp_path)

                    keep_samples = int(self.samplerate * 0.5)
                    audio_buffer = audio_buffer[-keep_samples:] if len(audio_buffer) > keep_samples else np.array([], dtype=np.float32)

            except Exception as e:
                print("İşleme hatası:", e)
                time.sleep(0.1)

    def resample(self, data, orig_sr, target_sr):
        if orig_sr == target_sr:
            return data
        from scipy.signal import resample
        number_of_samples = round(len(data) * float(target_sr) / orig_sr)
        return resample(data, number_of_samples)

# Callback fonksiyonu
processor = AudioProcessor()
def audio_callback(indata, outdata, frames, time, status):
    if status:
        print("hata", status)
    processor.audio_queue.put(indata.copy())
    outdata[:] = indata

# Thread başlat
threading.Thread(target=processor.process_audio, daemon=True).start()

# Yayın ve transkripsiyon başlat
try:
    with sd.Stream(device=(INPUT_DEVICE_INDEX, OUTPUT_DEVICE_INDEX),
                   samplerate=SAMPLERATE,
                   channels=CHANNELS,
                   dtype='float32',
                   callback=audio_callback):
        print("Yayın ve çeviri başladı. Çıkmak için Ctrl+C")
        while True:
            sd.sleep(1000)
except KeyboardInterrupt:
    print("Durduruldu.")
except Exception as e:
    print("Hata:", e)

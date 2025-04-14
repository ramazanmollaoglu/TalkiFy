
# 🔊 TalkiFy - Real-Time Subtitle Mirror

A TalkiFy overlay for Windows desktop powered by **Python**, **WPF**, and **Whisper AI**.

## 🎯 Project Overview

This project is a Windows application that listens to audio from a specific device (such as **VB-Audio Cable Input**) and mirrors live subtitle translations in both **English** and **Turkish** on top of your screen — similar to live captioning.

## 🚀 How It Works

1. **WPF UI**  
   - Launches a transparent, always-on-top panel (like subtitles).
   - Allows the user to choose:
     - **Input device** (e.g., *Cable Input*)
     - **Target output device**
     - **Baudrate** and **channel count**

2. **Audio Routing via VB-Cable**  
   - Requires [VB-Audio Virtual Cable](https://vb-audio.com/Cable/index.htm) to be installed.
   - Routes system sound into a virtual microphone.

3. **Python Transcription Backend**  
   - The WPF app starts a Python script in the background.
   - Python loads [OpenAI's Whisper](https://github.com/openai/whisper) speech recognition model.
   - Every 8 seconds, it segments the audio, transcribes it, and translates it into **English** and **Turkish**.
   - Sends the results back to the WPF window for real-time overlay.

## 🧰 Tech Stack

| Component | Technology |
|----------|------------|
| UI       | WPF (.NET) |
| Backend  | Python     |
| Audio    | VB-Cable   |
| ML       | Whisper (OpenAI) |
| Translate | `googletrans` (Google Translate API wrapper) |

## 📦 Dependencies

### Python
Install these dependencies with:

```bash
pip install -r requirements.txt
```

Or install manually:

- `whisper`
- `googletrans`
- `sounddevice`
- `numpy`
- `pyaudio` *(optional)*
- `torch` *(for Whisper)*

> ⚠️ You must have [VB-Cable Input](https://vb-audio.com/Cable/index.htm) installed to mirror system audio.

## 🖥️ Example Use Case

- You want to **watch a YouTube video in Turkish** but need **English subtitles**.
- You select your "Cable Output" as audio input, and headphones as output.
- The Python service transcribes + translates the speech live.
- The WPF overlay displays subtitles right over your video window.

## 📌 To Do

- [ ] Live microphone input
- [ ] Language auto-detection
- [ ] Adjustable refresh interval
- [ ] Whisper model selector (tiny → large-v2)

## 💡 Bonus Tips

- Works great for:
  - Language learners
  - Live translations during calls
  - Accessibility and hearing-impaired support

## 🪪 License
MIT

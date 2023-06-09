import os
import torch
import sys
import torchaudio

device = torch.device('cpu')
torch.set_num_threads(4)
local_file = 'model.pt'

if not os.path.isfile(local_file):
    torch.hub.download_url_to_file('https://models.silero.ai/models/tts/ru/v3_1_ru.pt',
                                   local_file)  

model = torch.package.PackageImporter(local_file).load_pickle("tts_models", "model")
model.to(device)

example_text = sys.argv[1]
filename = sys.argv[2]
b=bytes(filename,"cp866")
filename=str(b,"cp1251")
filename = filename + '.wav'
b=bytes(example_text,"cp866")
example_text=str(b,"cp1251")
sample_rate = 48000
speaker='kseniya'

audio = model.apply_tts(text=example_text,
                        speaker=speaker,
                        sample_rate=sample_rate)

torchaudio.save(filename,
                  audio.unsqueeze(0),
                  sample_rate=sample_rate)

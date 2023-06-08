cd "%~dp0SpeechRecognitionEnv\Scripts\"
call activate.bat
pip install soundfile
pip install omegaconf
pip3 install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu117
pause
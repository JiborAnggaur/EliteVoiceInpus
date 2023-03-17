set directory=%cd%
cd C:\Windows\system32
regsvr32.exe %directory%\Microsoft.Speech.dll
regsvr32.exe %directory%\Interop.AutoItX3Lib.dll
msiexec /a %directory%\MSSpeech_TTS_ru-RU_Elena.msi


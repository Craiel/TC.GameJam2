set git = "C:\Program Files (x86)\Git\bin\git.exe"
"C:\Program Files (x86)\Git\bin\git.exe" config pack.windowMemory 10m
"C:\Program Files (x86)\Git\bin\git.exe" config pack.packSizeLimit 20m
"C:\Program Files (x86)\Git\bin\git.exe" config core.compression 0
"C:\Program Files (x86)\Git\bin\git.exe" config core.loosecompression 0
rem "C:\Program Files (x86)\Git\bin\git.exe" gc
"C:\Program Files (x86)\Git\bin\git.exe" daemon --reuseaddr --base-path=. --export-all --verbose
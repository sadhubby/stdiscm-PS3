# stdiscm-s20-pset3

LAUNCH ORDER FOR TESTING:
1. Run Consumer (server) first → starts gRPC backend. Run dotnet run --project .\stdiscm-PS3.csproj
2. Run GUI → connects to Consumer and lists current uploads. Run dotnet run --project .\ConsumerGUI\ConsumerGUI.csproj
3. Run Producer(s) → uploads video files → Consumer saves them → GUI auto refresh shows new entries. Run dotnet run --project .\Producer\Producer.csproj

FOR GUI THUMBNAIL
- Go to https://www.gyan.dev/ffmpeg/builds/  
- download ffmpeg-release-essentials.zip
- update path, ex: string ffmpeg = @"D:\Users\user\Downloads\ffmpeg-8.0.1-essentials_build\ffmpeg-8.0.1-essentials_build\bin\ffmpeg.exe";


# stdiscm-s20-pset3

stdiscm-PS3/
│
├── Protos/                     ← shared protocol definitions
│   ├── greet.proto
│   └── streaming.proto
│
├── Shared/                     ← shared logic (helper classes)
│   ├── Models/
│   │   └── VideoMetadata.cs
│   └── Utils/
│       └── FileHelper.cs
│
├── Producer/                   ← uploader client
│   └── Producer.csproj
│
├── Consumer/                   ← backend receiver (server)
│   └── Consumer.csproj
│
├── ConsumerGUI/                ← GUI client (your component)
│   └── ConsumerGUI.csproj
│
└── stdiscm-PS3.sln             ← the Visual Studio solution file


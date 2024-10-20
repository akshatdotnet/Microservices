# Microservices

Design :

MicroservicesSolution/
├── src/
│   ├── ApiGateway
│   ├── AuthService/
│   │   ├── AuthService.Application/      # Application Layer
│   │   ├── AuthService.Domain/           # Domain Layer
│   │   ├── AuthService.Infrastructure/   # Infrastructure Layer
│   │   ├── AuthService.API/              # Presentation Layer (API)
│   ├── FilesService/
│   │   ├── FileService.Application/      # Application Layer
│   │   ├── FileService.Domain/           # Domain Layer
│   │   ├── FileService.Infrastructure/   # Infrastructure Layer
│   │   ├── FileService.API/              # Presentation Layer (API)
│   │   ├── FilesUpload Storage             
|   |       └── file-uploads/  # For storing uploaded files locally or Blob Storage
├── docker-compose.yml                    # Docker Compose for microservices orchestration
└── kubernetes/                           # Kubernetes configuration for microservices

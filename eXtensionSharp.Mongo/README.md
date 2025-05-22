# 📦 eXtensionSharp.Mongo

MongoDB를 .NET 프로젝트에서 타입 안전하고 선언적으로 사용할 수 있도록 도와주는 경량 래퍼 라이브러리입니다.  
`JMongo<T>`, `IJMongoFactory`, 인덱스 자동 초기화 기능을 지원하며, DI 기반으로 유연하게 구성할 수 있습니다.

---

## ✨ 주요 기능

- `JMongo<T>`: 컬렉션 접근을 제네릭 기반으로 단순화
- `IJMongoFactory`: 타입 기반으로 컬렉션 자동 생성
- `[JMongoCollection]`: 컬렉션 및 DB 정보 속성으로 선언
- `AddInitializer(...)`: 인덱스 정의를 코드 또는 구현체 기반으로 구성
- `UseJMongoDbAsync()`: 앱 시작 시 인덱스 자동 생성 실행

---

## 🔧 설치

```bash
dotnet add package MongoDB.Driver
```

> 📌 이 라이브러리는 MongoDB.Driver를 필요로 하며, .NET 6 이상에서 사용 가능합니다.

---

## 🧱 기본 구조

### 1. 모델 정의

```csharp
[JMongoCollection("AppDb", "sample_collection")]
public class SampleDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
}
```

---

### 2. 인덱스 초기화 클래스

```csharp
public class SampleDocumentIndexInitializer : IJMongoIndexInitializer
{
    public async Task InitializeIndexesAsync(IServiceProvider provider)
    {
        var factory = provider.GetRequiredService<IJMongoFactory>();
        var col = factory.Create<SampleDocument>().GetCollection();

        await col.Indexes.CreateOneAsync(
            new CreateIndexModel<SampleDocument>(
                Builders<SampleDocument>.IndexKeys.Ascending(x => x.CreatedAt),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(90) }
            )
        );
    }
}
```

---

### 3. DI 구성 (`Program.cs` 또는 `Startup.cs`)

```csharp
builder.Services.AddJMongoDb("mongodb://localhost:27017", options =>
{
    // 타입 기반 등록 (클래스는 DI 자동 등록됨)
    options.AddInitializer<SampleDocumentIndexInitializer>();

    // 또는 람다 방식으로 직접 등록도 가능
    options.AddInitializer(async provider =>
    {
        var factory = provider.GetRequiredService<IJMongoFactory>();
        var col = factory.Create<AnotherDocument>().GetCollection();

        await col.Indexes.CreateOneAsync(new CreateIndexModel<AnotherDocument>(
            Builders<AnotherDocument>.IndexKeys.Ascending(x => x.Timestamp)
        ));
    });
});
```

---

### 4. 앱 시작 시 인덱스 초기화 실행

```csharp
var app = builder.Build();

await app.Services.UseJMongoDbAsync(); // 인덱스 자동 생성

app.Run();
```

---

## 🧩 주요 인터페이스 요약

| 인터페이스 | 설명 |
|------------|------|
| `IJMongoFactory` | `T` 타입에 부여된 `JMongoCollectionAttribute`를 기반으로 컬렉션을 생성 |
| `IJMongoIndexInitializer` | 인덱스 생성 정의 인터페이스 |
| `IJMongoIndexInitializerRunner` | 등록된 초기화 작업들을 실행하는 실행기 |

---

## 📁 핵심 구성 요소

- `JMongo<T>`: 컬렉션 접근을 추상화
- `JMongoFactory`: 리플렉션 기반으로 컬렉션 설정 추출
- `JMongoDbOptions`: 인덱스 정의 수집 컨테이너
- `DependencyInjection`: 서비스 등록 및 인덱스 실행 메서드

---

## ✅ 라이선스

MIT License (자유롭게 사용 및 수정 가능)

---

## 👨‍💻 개발 기여

Pull Request 및 이슈 환영합니다.  
구조 개선, 기능 확장(예: Attribute 기반 Index 정의, Migration 지원 등) 제안도 적극 수용합니다.
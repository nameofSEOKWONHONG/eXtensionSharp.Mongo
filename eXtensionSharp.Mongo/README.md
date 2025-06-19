# 📦 eXtensionSharp.Mongo

MongoDB를 .NET 프로젝트에서 **EF Core 스타일의 선언적 구성 방식**으로 사용할 수 있게 해주는 경량 래퍼 라이브러리입니다.  
`JMongo<T>`, `IJMongoFactoryBuilder`, `IJMongoConfiguration<T>` 등을 통해 컬렉션 및 인덱스를 명시적으로 정의하고 자동 초기화합니다.

---

## ✨ 주요 기능

- `JMongo<T>`: 컬렉션 접근을 제네릭 기반으로 추상화
- `IJMongoFactory` / `IJMongoFactoryBuilder`: 타입 기반 컬렉션 생성을 선언적으로 구성
- `IJMongoConfiguration<T>`: 인덱스 및 컬렉션 정의를 코드로 설정 (속성/리플렉션 없음)
- `AddJMongoDb(...)` + `UseJMongoDb()`: DI 등록 및 앱 시작 시 자동 초기화

---

## 🔧 설치

```bash
dotnet add package MongoDB.Driver
```

> 📌 .NET 8 이상 권장, MongoDB.Driver 필요

---

## 🧱 사용 방법

### 1. 모델 정의

```csharp
public class SampleDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public string Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("age")]
    public int Age { get; set; }

    [BsonElement("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }
}
```

---

### 2. 컬렉션 및 인덱스 구성

```csharp
public class SampleDocumentConfiguration : IJMongoConfiguration<SampleDocument>
{
    public void Configure(JMongoBuilder<SampleDocument> builder)
    {
        builder.ToDocument("sample", "demo")
               .ToCollection(options =>
               {
                   options.Capped = false;
                   options.Collation = new Collation("en");
               })
               .ToIndex(indexes =>
               {
                   indexes.CreateOne(new CreateIndexModel<SampleDocument>(
                       Builders<SampleDocument>.IndexKeys.Ascending(x => x.CreatedAt),
                       new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(2) }
                   ));
               });
    }
}
```

---

### 3. DI 구성

```csharp
builder.Services.AddJMongoDb("mongodb://localhost:27017", options =>
{
    options.ApplyConfiguration(new SampleDocumentConfiguration());
});
```

---

### 4. 앱 시작 시 자동 초기화

```csharp
var app = builder.Build();

app.UseJMongoDb(); // 컬렉션 및 인덱스 생성

app.Run();
```

---

## 💡 선언적 설정이 필요한 이유

기존 MongoDB 사용 방식은 `IMongoClient.GetDatabase().GetCollection()` 호출이 반복되고, 인덱스도 외부에 흩어지기 쉽습니다.  
이 라이브러리는 **모델별 컬렉션 정의와 인덱스를 한 곳에 명시적으로 선언**하여 유지보수성과 테스트성을 강화합니다.

---

## 📁 주요 구성 요소

| 구성요소 | 설명 |
|---------|------|
| `JMongo<T>` | 컬렉션 접근 추상화 |
| `JMongoBuilder<T>` | 컬렉션 이름, 데이터베이스 이름, 인덱스, 생성 옵션 등 구성 |
| `IJMongoFactoryBuilder` | 구성된 빌더 등록자 |
| `IJMongoConfiguration<T>` | 타입별 컬렉션 구성 정의 인터페이스 |
| `JMongoDbOptions` | 구성 초기화 등록 컨테이너 |
| `DependencyInjection` | DI 등록 및 실행 확장 메서드 |

---

## 🧪 예제: 컬렉션 삽입

```csharp
var factory = provider.GetRequiredService<IJMongoFactory>();
var collection = factory.Create<SampleDocument>();

var doc = new SampleDocument { Name = "test", Age = 30, CreatedAt = DateTimeOffset.UtcNow };
await collection.InsertOneAsync(doc);
```

---

## ✅ 라이선스

MIT License (자유롭게 사용/수정 가능)

---

## 👨‍💻 기여

Pull Request, 기능 제안 모두 환영합니다.  
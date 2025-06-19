using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

public class JMongoCollectionBuilder<T> where T : class
{
    internal string DatabaseName { get; private set; }
    internal string CollectionName { get; private set; }
    internal CreateCollectionOptions<T> CreateOptions { get; private set; }


    private readonly List<Action<IMongoIndexManager<T>>> _indexActions = new();
    private bool _indexApplied = false;

    public JMongoCollectionBuilder<T> ToDocument(string dbName, string collectionName)
    {
        DatabaseName = dbName;
        CollectionName = collectionName;
        return this;
    }

    /// <summary>
    /// 📌 CreateCollectionOptions<T> 주요 속성 설명
    /// 속성	설명	사용 예
    /// Capped	고정 크기의 capped 컬렉션 여부 설정 (true면 오래된 문서 자동 삭제됨)	로그 기록 등 순환 저장 구조
    /// Collation	정렬/비교 기준 (대소문자 구분, 언어 등)	한글/영어 혼용 정렬, 대소문자 무시 비교
    /// ChangeStreamPreAndPostImagesOptions	Change Stream에서 변경 전/후 값을 캡처할지 여부 설정	CDC 기반 아키텍처에서 유용
    /// EncryptedFields	암호화 필드 메타 정의 (CSFLE 사용 시 필요)	민감 정보 암호화 저장 (e.g. 주민번호)
    /// ExpireAfter	TTL 방식으로 문서 만료 시간 지정	세션, 임시 인증코드, 캐시 데이터 자동 삭제
    /// IndexOptionDefaults	기본 인덱스 옵션 설정	자동 인덱스 생성 전략 일관화
    /// MaxDocuments	최대 문서 수 (Capped 컬렉션 전용)	고정 사이즈 저장소 제어
    /// MaxSize	컬렉션 최대 크기 바이트 단위 설정	디스크 사용량 제한
    /// NoPadding	레거시 storage padding 비활성화 (MongoDB 5.x 이상에서는 거의 사용 안함)	일반적으로 사용 안함
    /// StorageEngine	특정 스토리지 엔진 설정 (e.g. WiredTiger 옵션 조정)	고급 환경에서 엔진 커스터마이징
    /// TimeSeriesOptions	시계열 컬렉션 생성 시 필수 설정	IoT 센서, 로그 등 시간 기반 데이터
    /// UsePowerOf2Sizes	deprecated. padding 사용 여부 (MongoDB 3.0 이후 의미 없음)	사용 안함 권장
    /// ValidationAction	문서 유효성 검사 실패 시의 처리 (Warn, Error)	데이터 무결성 강화
    /// ValidationLevel	유효성 검사의 적용 수준 (Strict, Moderate, Off)	마이그레이션 중 점진적 검증 등
    /// ClusteredIndex	Clustered 인덱스 설정 (MongoDB 5.3+)	PK 기반 저장 최적화 시 사용
    /// DocumentSerializer	커스텀 직렬화기 지정	복잡한 구조 직렬화 또는 DTO ↔ Entity 간 매핑
    /// Validator	문서의 JSON Schema 또는 조건 설정	예: { age: { $gt: 18 } } 등
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public JMongoCollectionBuilder<T> ToCollection(Action<CreateCollectionOptions<T>> configure)
    {
        var options = new CreateCollectionOptions<T>();
        configure(options);
        CreateOptions = options;
        return this;
    }

    public JMongoCollectionBuilder<T> ToIndex(Action<IMongoIndexManager<T>> indexManager)
    {
        _indexActions.Add(indexManager);
        return this;
    }

    internal void ApplyIndexes(IMongoCollection<T> collection)
    {
        if (_indexApplied)
            return;

        foreach (var action in _indexActions)
        {
            action(collection.Indexes);
        }

        _indexApplied = true;
    }
}
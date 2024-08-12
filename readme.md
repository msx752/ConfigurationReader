# ConfigurationLib Doküman

#### **Proje Açıklaması:**
ConfigurationLib, .NET tabanlı uygulamalar için yapılandırma ayarlarını merkezi bir şekilde yönetmek ve çeşitli veri kaynaklarından bu ayarları dinamik olarak almak amacıyla geliştirilmiş bir kütüphanedir. Kütüphane, özellikle MongoDB gibi veri kaynakları ile etkileşimde bulunarak, uygulamaların yapılandırma ayarlarını veritabanlarından okur ve günceller. Aynı zamanda, esneklik sağlamak için Polly gibi araçlarla entegre edilebilecek şekilde tasarlanmıştır.

### Teknolojiler

- **.NET 5.0**: Projenin temel geliştirme platformudur.
- **MongoDB**: Yapılandırma verilerini saklamak ve yönetmek için kullanılan NoSQL veritabanıdır.
- **Polly**: Hata yönetimi ve esneklik stratejileri için kullanılan bir .NET kütüphanesidir. Özellikle circuit breaker politikaları için entegre edilmiştir.
- **Docker**: Uygulamanın farklı ortamlarda çalıştırılabilmesi için konteynerizasyonu sağlayan bir araçtır.
- **Docker Compose**: Birden fazla Docker konteynerini yönetmek için kullanılır. Proje, MongoDB gibi servislerle birlikte çalışmak üzere Docker Compose ile yapılandırılmıştır.
- **xUnit**: Projede birim testleri yazmak için kullanılan test framework'üdür.
- **Moq**: Bağımlılıkların ve servislerin davranışlarını taklit etmek için kullanılan mocking framework'üdür.
- **coverlet.collector**: Kod kapsama oranını ölçmek ve raporlamak için kullanılan araçtır.

### **Gereksinimler:**
- .NET 5.0 SDK
- Docker ve Docker Compose yüklü olmalı
- MongoDB sunucusu (Docker ile çalıştırılabilir)

## 1. Sınıflar ve Metotlar

### 1.1 `ApplicationConfiguration` Sınıfı

#### **Amaç:**
`ApplicationConfiguration` sınıfı, uygulama yapılandırma ayarlarını temsil eder. Bu sınıf, bir yapılandırma öğesinin kimliğini, adını, türünü, değerini ve etkin olup olmadığını içerir.

#### **Özellikler:**
- **`Id`**: Yapılandırma öğesinin benzersiz kimliğini tutar.
- **`Name`**: Yapılandırma öğesinin adını tutar.
- **`Type`**: Yapılandırma öğesinin türünü belirten string değeri tutar.
- **`Value`**: Yapılandırma öğesinin değerini tutar.
- **`IsActive`**: Yapılandırma öğesinin aktif olup olmadığını belirten boolean değeri tutar.
- **`ApplicationName`**: Yapılandırma öğesinin hangi uygulama için geçerli olduğunu belirtir.

### 1.2 `IConfigurationReader` Arayüzü

#### **Amaç:**
`IConfigurationReader` arayüzü, yapılandırma okuyucuları için bir kontrat tanımlar. Bu arayüz, yapılandırma ayarlarının alınmasını ve belirli bir anahtara göre değerlerin elde edilmesini sağlayan metodları tanımlar.

#### **Metotlar:**
- **`GetValue<T>(string key)`**: Belirtilen anahtar ile yapılandırma değerini döner. 
  - **Parametreler**: 
    - `key`: Yapılandırma değerinin anahtarı.

### 1.3 `ConfigurationReaderBase` Sınıfı

#### **Amaç:**
`ConfigurationReaderBase`, yapılandırma okuyucuları için temel bir sınıftır. Bu sınıf, temel işlevleri ve yapılandırma koleksiyonlarını yönetir.

#### **Özellikler:**
- **`_circuitBreakerPolicy`**: Circuit breaker politikası.
- **`_configurationCollection`**: Yapılandırma verilerini saklayan koleksiyon.
- **`_refreshTimerIntervalInMs`**: Yapılandırma yenileme zamanlayıcısının milisaniye cinsinden aralığı.
- **`_timer`**: Zamanlayıcı, yapılandırma ayarlarını düzenli olarak yeniler.
- **`_disposedValue`**: Sınıfın serbest bırakılma durumu.
- **`Collection`**: Yapılandırma koleksiyonunu döner.
- **Overlapping** kontrolü, aktif bir işlem olduğunda thread-safe bir yöntem ile 2.tetiklemeyi **_refreshTimerIntervalInM**s değeri kadar öteler.
  
#### **Metotlar:**
- **`GetValue<T>(string key)`**: Belirtilen anahtara karşılık gelen değeri döner. Hatalar:
  - **ArgumentNullException**
  - **NotSupportedException**
  - **ArgumentException**
  - **Döndürülen Değer**: Generic türde bir değer döner.

### 1.4 `MongoDbConfigurationReader` Sınıfı

#### **Amaç:**
`MongoDbConfigurationReader`, MongoDB veritabanından yapılandırma ayarlarını almak için kullanılan bir sınıftır. `ConfigurationReaderBase` sınıfından türetilmiştir ve MongoDB ile etkileşim kurar.

#### **Özellikler:**
- **`_connectionString`**: MongoDB bağlantı dizesi.
- **`_database`**: MongoDB veritabanı.
- **`_collection`**: MongoDB koleksiyonu.

#### **Metotlar:**
- **`InitializeMongoClient(string connectionString)`**: MongoDB istemcisini başlatır.
- **`ListConfigurationByApplicationNameAsync(string applicationName)`**: Belirtilen uygulama adına göre yapılandırma ayarlarını listeleyen asenkron metod.
  - **Parametreler**:
    - `applicationName`: Yapılandırma ayarlarının alınacağı uygulama adı.
  - **Döndürülen Değer**: Asenkron görev olarak yapılandırma öğelerinin listesi.

### 1.5 `ConfigurationReader` Sınıfı

#### **Amaç:**
`ConfigurationReader`, belirli bir uygulama için yapılandırma ayarlarını yöneten sınıftır. Bu sınıf, bir uygulama adına göre yapılandırma ayarlarını alır ve yönetir.

#### **Özellikler:**
- **`_applicationName`**: Uygulama adı.
- **`_connectionString`**: Veritabanı bağlantı dizesi.
- **`_refreshTimerIntervalInMs`**: Yenileme aralığı.

#### **Metotlar:**
- **`ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs)`**: Sınıfın yeni bir örneğini başlatır.

### 1.6 `Extensions` Sınıfı

#### **Amaç:**
`Extensions` sınıfı, çeşitli uzantı metotları sağlar. Bu metotlar, yapılandırma yönetimi ile ilgili çeşitli işlemleri kolaylaştırır.

#### **Metotlar:**
- **`IsSupportedType(Type type)`**: Belirtilen bir türün desteklenip desteklenmediğini kontrol eder.
- **`GetSupportedTypeByStringType(string stringType)`**: Bir string türüne karşılık gelen desteklenen türü döner.


## 2. Kullanım Örneği: Dependency Injection (DI)

### 2.1 Dependency Injection (DI) ile IConfigurationReader Ekleme

Aşağıda, `IConfigurationReader` arayüzünü Dependency Injection (DI) konteynerine eklemek için bir örnek verilmiştir. Bu örnekte, `ConfigurationReader` sınıfı kullanılarak MongoDB bağlantı dizesi, uygulama adı ve yenileme zamanlayıcı aralığı yapılandırılır.

```csharp
services.AddSingleton<IConfigurationReader>(x => 
    new ConfigurationReader(
        Configuration["AppName"], 
        Configuration.GetConnectionString("MongoDbConnectionString"), 
        Configuration.GetValue<int>("RefreshTimerIntervalInMs")
    ));
```

## Shared Kernel

### 1. `MongoDbContext` Servisi

#### **Amaç:**
`MongoDbContext`, MongoDB veritabanı ile etkileşimi sağlayan bir bağlam sınıfıdır. Bu sınıf, MongoDB koleksiyonlarına erişimi ve yönetimini kolaylaştırır. Uygulamadaki veri erişimi işlemlerinin merkezi bir noktadan yönetilmesi için kullanılır.

#### **Özellikler ve Metotlar:**
- **`Database`**: MongoDB veritabanı nesnesini temsil eder.
- **`GetCollection<T>(string name)`**: Verilen isimle bir MongoDB koleksiyonu döner. Bu, belirli bir veri türü ile ilişkili koleksiyona erişim sağlar.
- **`Dispose()`**: Veritabanı bağlamını serbest bırakır ve kaynakları temizler.

### 2. `MessageBus` Servisi

#### **Amaç:**
`MessageBus`, RabbitMQ gibi mesajlaşma altyapıları ile etkileşimi sağlayan bir servistir. Bu servis, uygulama içi veya mikroservisler arası iletişimde mesaj kuyruğuna mesaj gönderme ve alma işlemlerini yönetir. Bu yapı, sistemin esnek ve ölçeklenebilir olmasını sağlar.

#### **Özellikler ve Metotlar:**
- **`Publish<T>(T message, string queueName)`**: Belirtilen kuyruk adına bir mesaj yayınlar.
  - **Parametreler:**
    - `message`: Gönderilecek mesaj.
    - `queueName`: Mesajın gönderileceği kuyruk adı.
- **`Subscribe<T>(string queueName, Action<T> onMessage)`**: Belirtilen kuyruk adına gelen mesajları dinler ve bir işlem gerçekleştirir.
  - **Parametreler:**
    - `queueName`: Dinlenecek kuyruk adı.
    - `onMessage`: Mesaj alındığında gerçekleştirilecek işlem.

### 3. `ExceptionLogEvent` ve `ExceptionLog` Sınıfları

#### **Amaç:**
`ExceptionLogEvent` ve `ExceptionLog` sınıfları, uygulama genelinde meydana gelen istisnaların kaydedilmesi ve yönetilmesi için kullanılır. Bu sınıflar, hata günlükleme mekanizmasının temelini oluşturur. `ExceptionLogEvent` her bir hata olayı için detayları içerirken, `ExceptionLog` bu olayların kaydedildiği genel log yapısını temsil eder.

#### **`ExceptionLogEvent` Sınıfı:**
- **Özellikler:**
  - **`EventId`**: İlgili istisna olayının benzersiz kimliği.
  - **`Message`**: İstisna mesajı.
  - **`StackTrace`**: İstisna sırasında oluşan stack trace bilgisi.

#### **`ExceptionLog` Sınıfı:**
- **Özellikler:**
  - **`LogId`**: Günlüğe kaydedilen istisnanın benzersiz kimliği.
  - **`LoggedDate`**: İstisnanın kaydedildiği tarih ve saat.
  - **`ExceptionLogEvents`**: İlgili istisna olaylarının listesi.

### 4. `ApplicationConfigurationRepository` ve `ExceptionLogRepository` Sınıfları

#### **Amaç:**
Bu sınıflar, veri erişim katmanını temsil eder ve MongoDB üzerindeki koleksiyonlarla etkileşimi sağlar. Her iki sınıf da belirli veri setlerini yönetir ve bunları uygulamanın diğer katmanlarına sunar.

#### **`ApplicationConfigurationRepository` Sınıfı:**
- **`GetApplicationConfiguration(string applicationName)`**: Belirtilen uygulama adına göre yapılandırma ayarlarını alır. Bu metod, uygulamanın çalışması için gerekli yapılandırma ayarlarını MongoDB'den getirir.

#### **`ExceptionLogRepository` Sınıfı:**
- **`LogException(ExceptionLog exceptionLog)`**: Bir istisna olayını MongoDB'ye kaydeder. Bu metod, uygulamada meydana gelen hataların merkezi bir noktada toplanmasını ve yönetilmesini sağlar.

## Dashboard UI ve LogService Projeleri

### **1. Proje Tanımları**

**Dashboard UI:**
- Merkezi bir kullanıcı arayüzü olup, servislerin yapılandırma ayarlarının yönetildiği yerdir.
- Yapılandırma ekleme, aktif/pasif hale getirme ve hataların listelenmesi işlemleri yapılır.
- Hata loglama için `IsLoggingEnabled` değeri `true` olmalıdır ve `ApplicationName` değeri **Dashboard** olmalıdır.
- Yapılandırma ayarları her 3 saniyede bir yenilenir.
- Servis URL'i: http://localhost:50400
- **Örnek Konfigürasyonlar:**
  - `int MaxItemCount`
  - `boolean IsBasketEnabled`
  - `string SiteName`

**LogService (ServiceLog):**
- Uygulama hatalarını loglar ve RabbitMQ üzerinden gelen hata mesajlarını MongoDB'ye kaydeder.
- Hata mesajları işlendiğinde log atmak için `ApplicationName` değeri **ServiceLog** olmalıdır.
- Yapılandırma ayarları her 5 saniyede bir yenilenir.
- Servis URL'i: http://localhost:50600/Configuration

### **2. Teknolojiler**

- **MongoDB**: Yapılandırma ve hata loglarının saklanması için kullanılır.
- **RabbitMQ**: Hata mesajlarını almak ve işlemek için kullanılır.

### **3. Veri Akışı ve İzleme**

1. **Dashboard UI:**
   - Yapılandırma ayarlarını 3 saniyede bir yeniler.
   - `IsLoggingEnabled` değeri `true` olduğunda hata loglama yapılır.
   - **Örnek Konfigürasyon Değişiklikleri:**
     - `MaxItemCount` değeri değiştirilebilir.
     - `IsBasketEnabled` değeri etkinleştirilebilir/pasifleştirilebilir.
     - `SiteName` değeri değiştirilebilir.

2. **LogService (ServiceLog):**
   - RabbitMQ’dan gelen hata mesajlarını 5 saniyede bir yenilenen ayarlarla işler ve MongoDB’ye kaydeder.
   - Hata mesajı işlendiğinde, log atmak için `ApplicationName` değeri **ServiceLog** olarak ayarlanmalıdır.

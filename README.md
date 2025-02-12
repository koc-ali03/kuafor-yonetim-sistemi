# Kuaför/Berber İşletme Yönetim Sistemi

Bu proje, ASP.NET Core MVC teknolojisi kullanılarak geliştirilmiş bir kuaför/berber işletme yönetim sistemidir. Sistem, salonların işlemlerini, çalışanların müsaitlik durumlarını ve randevu yönetimini etkin bir şekilde takip etme ve yönetme imkanı sunar. Ayrıca, kullanıcılar için yapay zeka destekli saç modeli ve renk önerileri sağlar.

> [!NOTE]
> Projede yapay zeka özelliği için API anahtarı gerekmektedir, ekleme talimatları için [buraya tıklayın](#api-anahtarı-ekleme).

## Proje Özellikleri

### 1. Kuaför/Berber Tanımlamaları
- Kuaför ve berber salonlarının çalışma saatleri, sunduğu işlemler ve bu işlemlerin süre/ücret bilgileri tanımlanabilir.

### 2. Çalışan Yönetimi
- Çalışanların uzmanlık alanları ve yapabilecekleri işlemler belirlenebilir.
- Çalışanların uygunluk saatleri girilerek kullanıcıların bu saatlere göre randevu alması sağlanır.

### 3. Randevu Sistemi
- Kullanıcılar, uygun çalışanlar ve işlemler doğrultusunda randevu alabilir.
- Randevu saatleri kontrol edilerek çakışmalar önlenir.
- Randevu detayları (işlem, süre, ücret) sistemde saklanır ve onay mekanizması bulunur.

### 4. REST API Kullanımı
- Projenin bazı bölümlerinde veri tabanı ile iletişim REST API aracılığıyla sağlanmaktadır.

### 5. Yapay Zeka Entegrasyonu
- Kullanıcılar fotoğraf yükleyerek yapay zeka sayesinde saç modeli veya renk önerileri alabilir.

## Kullanılan Teknolojiler
- **Backend**: ASP.NET Core 8 MVC
- **Frontend**: HTML5, CSS3, Bootstrap, JavaScript, jQuery
- **Veritabanı**: PostgreSQL
- **ORM**: Entity Framework Core
- **Ek Özellikler**: REST API, Yapay Zeka Entegrasyonu

## API Anahtarı Ekleme
Yapay zeka destekli saç modeli ve renk önerileri özelliğini kullanmak için API anahtarını yapılandırmanız gerekir. Aşağıdaki adımları takip edin:
```bash
dotnet user-secrets init
dotnet user-secrets set "APIKeys:HairstyleAPI" "API-Anahtarınız-Buraya"
```
Bu anahtar, proje çalıştırıldığında Secret Manager üzerinden otomatik olarak alınır.

## Katkıda Bulunanlar
- Bedirhan Can
- Ali Koç

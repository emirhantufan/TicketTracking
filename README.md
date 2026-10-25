# Basit Hata ve Talep Takip Sistemi (TicketTracking)

ASP.NET Core MVC mimarisi ile geliştirilmiş; kullanıcı bazlı yetkilendirme, durum takibi, yorum akışı, filtreleme, dosya ekleme ve grafiksel gösterge paneli (dashboard) özelliklerine sahip hata ve talep yönetim platformu.

---

## 🚀 Kullanılan Teknolojiler

- **Backend:** C#, ASP.NET Core MVC (.NET 9.0)
- **Veritabanı & ORM:** Microsoft SQL Server / LocalDB, Entity Framework Core (Code-First)
- **Kimlik Doğrulama:** Cookie Authentication & Claims-Based Identity
- **Frontend & Arayüz:** Bootstrap 5, Bootstrap Icons, HTML5, CSS3, JavaScript
- **Veri Görselleştirme:** Chart.js
- **Mimari:** Standart MVC Deseni, ViewModel & Separation of Concerns

---

## 📌 Proje Özellikleri ve Yetenekleri

1. **Kullanıcı & Oturum Yönetimi:**
   - Güvenli parola saklama, kayıt olma (Register) ve oturum açma (Login).
   - Sayfa erişim kısıtlamaları (`[Authorize]`).
2. **Proje Yönetimi (Project CRUD):**
   - Proje ekleme, listeleme, düzenleme ve silme.
3. **Talep & Hata Yönetimi (Ticket CRUD):**
   - Başlık, açıklama, proje, talep türü (Hata/Talep/Soru) ve öncelik (Düşük/Orta/Yüksek) seçimiyle talep oluşturma.
   - Form veri doğrulamaları (Data Annotations) ile karakter ve zorunluluk kontrolleri.
   - Talep kapatma iş kuralı (kapatılan taleplerin düzenlenememesi).
4. **Sahiplik Yetkilendirmesi (Ownership Authorization):**
   - Kullanıcıların sadece kendi oluşturdukları talepleri düzenleyebilmesi ve kapatabilmesi.
   - URL manipülasyonu ile başkasının kaydına erişim girişimlerinin sunucu tarafında engellenmesi.
5. **Yorum Sistemi:**
   - Bilet detayında anlık yorum yazabilme, yorum sahibi ve zaman damgası takibi.
6. **Dosya Yükleme (Ticket Attachments):**
   - Taleplere ekran görüntüsü ve ek dosya yükleyebilme, doğrudan tarayıcıdan önizleme/indirme.
7. **Dinamik Filtreleme:**
   - Bilet listesinde Tür, Öncelik, Durum ve Proje kriterlerine göre çoklu süzme.
8. **Gösterge Paneli (Dashboard & Raporlama):**
   - Toplam, açık, kapatılan talep ve proje sayaç kartları.
   - Chart.js ile Durum Dağılımı (Doughnut) ve Tür Dağılımı (Bar) grafikleri.
   - Son eklenen talepler özeti.

---

## ⚙️ Kurulum ve Çalıştırma

### Gereksinimler
- .NET 8.0 veya .NET 9.0 SDK
- Microsoft SQL Server LocalDB
- Visual Studio Code veya Visual Studio 2022

### Kurulum Adımları

1. **Depoyu Klonlayın:**
   ```bash
   git clone [https://github.com/emirhantufan/TicketTracking.git](https://github.com/emirhantufan/TicketTracking.git)
   cd TicketTracking

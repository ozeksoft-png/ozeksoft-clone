# C# + DevExpress ile Yedekleme Programı

Bu örnek proje, DevExpress WinForms bileşenleriyle bir yedekleme arayüzü ve arka planda çalışan temel bir yedekleme servisinden oluşur.

## İçerik
- `Backup.Core`: Yedekleme planı ve dosya kopyalama/zip işlemleri.
- `Backup.UI`: DevExpress tabanlı WinForms arayüzü.

## Gereksinimler
- .NET 6 SDK
- DevExpress WinForms bileşenleri (lisanslı)

## Kurulum
1. DevExpress paketlerini ekleyin (ör. `DevExpress.Win` veya lisansınıza uygun paketler).
2. `Backup.UI` projesini varsayılan proje olarak ayarlayın.
3. Uygulamayı başlatın.

## Kullanım
- Plan adı, kaynak klasör ve hedef klasörü seçin.
- Zip ile sıkıştırma tercihini ve saklama sayısını belirleyin.
- **Yedekle** butonuna basın.

## Notlar
- `Backup.Core` içindeki `RetentionCount` en yeni N yedeği tutacak şekilde çalışır.
- DevExpress bileşenleri eklenmeden derleme yapılırsa `Backup.UI` projesi hata verir.

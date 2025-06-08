# Workshop Manager

Projekt jest aplikacją webową umożliwiającą zarządzanie warsztatem samochodowym. Dostępne są 4 role dla użytkowników:

### Admin
- Może przeglądać listę użytkowników, ich pojazdy i zlecenia, oraz zmieniać ich role
- Może tworzyć konta dla mechaników i recepcjonistów
- Może przeglądać i zarządzać katalogiem części
- Może wygenerować miesięczny raport zleceń, oraz otrzymywać na maila raport o obecnych zleceniach

### Recepcjonista
- Może zarządzać klientami, ich pojazdami i zleceniami
- Może również zarządzać katalogiem części

### Mechanik
- Może przeglądać listę zleceń oraz ich szczegóły
- Może sporządać listę wykonanych zadań dla danego zlecenia i zużytych części

### Klient
- Może przeglądać swoje pojazdy oraz stan ich napraw

Aby się zalogować, użytkownik musi podać nazwę użytkownika (email) oraz hasło

# W projekcie wykorzystano

### ASP.NET Core
- Backend

### Razor Pages
- Frontend

### Nlog
- Służy do bardziej zorganizowanego zbierania logów z działania aplikacji

### NBomber
- Umożliwia symulowanie ruchu wielu użytkowników, w celu testów obciążeniowych

### Github Actions
- Sprawdza czy aplikacja poprawnie się kompiluje i czy działają testy

### BackgroundService
- Wysyła adminom codzienne raporty z obecnych zleceń

### SQL Server
- Baza danych

### SQL Profiler
- Umożliwia monitorowanie działania bazy danych
  

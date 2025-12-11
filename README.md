# LessonLink

Tanórafoglaló ewbalkalmazás magántanárok és diákok részére

## Tartalomjegyzék

- [Áttekintés](#áttekintés)
- [Technológiai stack](#technológiai-stack)
- [Előfeltételek](#előfeltételek)
- [Futtatási útmutató](#futtatási-útmutató)
- [Használat](#használat)
- [Tesztelés](#tesztelés)

## Áttekintés

A LessonLink egy oktatási foglalási rendszer, amely lehetővé teszi tanárok és diákok számára az órafoglalások egyszerű kezelését.

## Technológiai stack

### Backend
- **.NET 9.0** - Web API
- **ASP.NET Core Identity** - Felhasználókezelés
- **Entity Framework Core** - ORM
- **MS SQL Server** - Adatbázis
- **JWT Bearer** - Autentikáció

### Frontend
- **React 19** - UI framework
- **TypeScript** - Típusbiztonság
- **Vite** - Build tool
- **Material-UI** - Komponens könyvtár
- **React Query** - Szerver state menedzsment
- **Axios** - HTTP kliens
- **React Router** - Routing

## Előfeltételek

- **.NET 9.0 SDK**
  - Letöltés: https://dotnet.microsoft.com/download/dotnet/9.0
- **Node.js 20.x vagy újabb** és **npm**
  - Letöltés: https://nodejs.org/
- **MS SQL Server 2022** vagy **SQL Server Express**
  - Letöltés: https://www.microsoft.com/sql-server/sql-server-downloads
  - Alternatíva: SQL Server LocalDB (Visual Studio-val települ)

---

## Futtatási útmutató

### 1. Adatbázis előkészítése

#### SQL Server LocalDB használata (Windows):

SQL Server LocalDB általában Visual Studio-val települ. Ellenőrizze:
```powershell
sqllocaldb info
```

Ha nincs telepítve, töltse le innen: https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb

#### SQL Server Express használata:

1. Töltse le és telepítse: https://www.microsoft.com/sql-server/sql-server-downloads
2. Telepítés során válassza az "alapvető" (basic) telepítést
3. Jegyezze fel a szerver nevét (pl. `localhost\SQLEXPRESS` vagy `.\SQLEXPRESS`)

### 2. Backend futtatása

1. Navigáljon a backend könyvtárba:
   ```powershell
   cd LessonLink\LessonLink.WebApi
   ```

2. Állítsa be a connection string-et az `appsettings.json` fájlban:
   
   **LocalDB esetén** (Windows):
   ```json
   "ConnectionStrings": {
     "LessonLinkDb": "Server=(localdb)\\MSSQLLocalDB;Database=LessonLinkDb;Trusted_Connection=True;"
   }
   ```
   
   **SQL Server Express esetén**:
   ```json
   "ConnectionStrings": {
     "LessonLinkDb": "Server=localhost\\SQLEXPRESS;Database=LessonLinkDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

3. Futtassa a migrációkat:
   ```powershell
   dotnet ef database update --project ..\LessonLink.Infrastructure\LessonLink.Infrastructure.csproj
   ```

4. Indítsa el a backend-et:
   ```powershell
   dotnet run
   ```

5. A backend elindul és kiírja, hogy melyik porton fut. A terminálban keresse a következő sort:
   ```
   Now listening on: http://localhost:5250
   ```
   
   **Fontos:** Jegyezze meg ezt a portot (jelen esetben 5250), mert szükség lesz rá a frontend konfigurálásához!
   
   A Swagger API dokumentáció elérhető: `http://localhost:5250/swagger`

### 3. Frontend futtatása

1. Nyisson egy új terminált és navigáljon a frontend könyvtárba:
   ```powershell
   cd lessonlink-react-app
   ```

2. Telepítse a függőségeket:
   ```bash
   npm install
   ```

3. **Állítsa be a backend URL-t** a `vite.config.ts` fájlban:
   
   Nyissa meg a `lessonlink-react-app/vite.config.ts` fájlt és módosítsa a `target` értéket az előző lépésben látott backend portra:
   
   ```typescript
   server: {
     proxy: {
       '/api': {
         target: 'http://localhost:5250',  // <-- Itt írja be a backend portját!
         changeOrigin: true,
         secure: false,
       }
     }
   }
   ```

4. Indítsa el a fejlesztői szervert:
   ```bash
   npm run dev
   ```

5. A frontend elindul: http://localhost:5173

### 4. Teszt adatok

Az alkalmazás első indításkor automatikusan feltölti az adatbázist teszt adatokkal.

**Teszt felhasználók megtekintése:**

A teszt felhasználók adatait a következő fájlokban találja:
- Diákok: `LessonLink/LessonLink.Infrastructure/Seed/SeedData/StudentSeedData.json`
- Tanárok: `LessonLink/LessonLink.Infrastructure/Seed/SeedData/TeacherSeedData.json`

Minden teszt felhasználó jelszava: **Pa$$w0rd**

---

## Tesztelés

Az alkalmazás unit teszteket tartalmaz a `LessonLink.Test` projektben.

### Tesztek futtatása

```powershell
cd LessonLink;
dotnet test
```

---

## Megjegyzések

- Ha szeretné használni a képfeltöltés funkciót, az `appsettings.json`-ben a CloudinarySettings kulcshoz írja be azokat az értékeket, amiket kap a https://cloudinary.com oldalon. Itt regisztrálnia kell, azonban ez ingyenes és gyors.
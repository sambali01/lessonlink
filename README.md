# LessonLink

Tanórafoglaló webalkalmazás magántanárok és diákok részére.

## Tartalomjegyzék

- [Áttekintés](#áttekintés)
- [Technológiai stack](#technológiai-stack)
- [Előfeltételek](#előfeltételek)
- [Futtatási útmutató](#futtatási-útmutató)
- [Használat](#használat)
- [Tesztelés](#tesztelés)

## Technológiai stack

### Backend
- **.NET 9.0**
- **ASP.NET Core Identity**
- **Entity Framework Core**
- **MS SQL Server**
- **JWT Bearer**

### Frontend
- **React**
- **TypeScript**
- **Vite**
- **Material-UI**
- **React Query**
- **Axios**
- **React Router**

## Előfeltételek

- **.NET 9.0 SDK**
  - Letöltés: https://dotnet.microsoft.com/download/dotnet/9.0
  
- **Node.js 20.x vagy újabb** és **npm**
  - Letöltés: https://nodejs.org/
  
- **MS SQL Server** (az alábbiak egyike):
  - **SQL Server 2022** (Developer vagy Express Edition)
    - Letöltés: https://www.microsoft.com/sql-server/sql-server-downloads
  - **SQL Server LocalDB** (Windows, Visual Studio-val települ)
    - Alternatív letöltés: https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb
  - **SQL Server Express**
    - Letöltés: https://www.microsoft.com/sql-server/sql-server-downloads

---

## Futtatási útmutató

### 1. Adatbázis előkészítése

Válasszon az Előfeltételek között felsorolt SQL Server verziók közül és győződjön meg róla, hogy fut:

#### SQL Server 2022 (teljes verzió):

Ellenőrizze, hogy a SQL Server szolgáltatás fut (SQL Server Configuration Manager vagy Services.msc).

#### SQL Server LocalDB (Windows):

Ellenőrizze, hogy telepítve van és elérhető:
```powershell
sqllocaldb info
```

#### SQL Server Express:

Ellenőrizze, hogy a SQL Server (SQLEXPRESS) szolgáltatás fut (Services.msc).

### 2. Backend futtatása

1. Nyissa meg az `appsettings.json` fájlt a `LessonLink\LessonLink.WebApi` mappában és állítsa be a connection string-et:
   
   **LocalDB esetén** (Windows) - ez van alapértelmezetten beállítva:
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
   
   **SQL Server 2022 esetén**:
   ```json
   "ConnectionStrings": {
     "LessonLinkDb": "Server=localhost;Database=LessonLinkDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```
   
   Vagy ha SQL Server Authentication-t használ (felhasználónév/jelszó):
   ```json
   "ConnectionStrings": {
     "LessonLinkDb": "Server=localhost;Database=LessonLinkDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
   }
   ```

2. Navigáljon a projekt gyökérkönyvtárába:
   ```powershell
   cd LessonLink
   ```
3. Fordítsa le a projektet:
   ```powershell
   dotnet build
   ```

4. Navigáljon a backend könyvtárba:
   ```powershell
   cd LessonLink.WebApi
   ```

5. Indítsa el a backend-et:
   ```powershell
   dotnet run
   ```
   
   **Az alkalmazás automatikusan:**
   - Létrehozza az adatbázist (ha még nem létezik)
   - Lefuttatja az összes migrációt
   - Feltölti az adatbázist teszt adatokkal
   
   A siker jele, ha a konzolon ezt látja:
   ```
   All seed data successfully loaded!
   info: Microsoft.Hosting.Lifetime[14]
         Now listening on: http://localhost:5250
   ```

   **Fontos:** Jegyezze meg ezt a portot (a fenti példában 5250), mert szükség lesz rá a frontend konfigurálásához!

5. A backend elindul és kiírja, hogy melyik porton fut.
   
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
                target: 'http://localhost:5250', // <- itt írja be
                changeOrigin: true,
                secure: false,
                ws: true
            },
        },
    },
   ```

4. Indítsa el a fejlesztői szervert:
   ```bash
   npm run dev
   ```

5. A frontend elindul: http://localhost:5173

### 4. Kezdeti adatok

Az alkalmazás első indításkor automatikusan feltölti az adatbázist adatokkal.

Ezeknek felhasználóknak az adatait a következő fájlokban találja:
- Diákok: `LessonLink/LessonLink.Infrastructure/Seed/SeedData/StudentSeedData.json`
- Tanárok: `LessonLink/LessonLink.Infrastructure/Seed/SeedData/TeacherSeedData.json`

Itt megtalálja a bejelentkezéshez szükséges email címeket.

Minden felhasználó jelszava: **Pa$$w0rd**

---

## Tesztelés

Az alkalmazás unit teszteket tartalmaz a `LessonLink.Test` projektben. Ezeket a következő parancsokkal futtathatja:

```powershell
cd LessonLink
```

```powershell
dotnet test
```

---

## Opcionális: Képfeltöltés funkció beállítása

Az alkalmazás lehetővé teszi tanári profilképek feltöltését. Ez a funkció csak Cloudinary fiókkal állítható be, ami az alábbi lépésekkel aktiválható:

### 1. Cloudinary fiók létrehozása

1. Látogasson el a https://cloudinary.com oldalra
2. Kattintson a **Sign Up for Free** gombra és regisztráljon be

### 2. API kulcsok megszerzése

1. Jelentkezzen be a Cloudinary fiókjába
2. Menjen be bal oldalon a **Product environment settings** alatt az **API Keys** menüpontba
3. Kattintson jobb felül a **Generate New API Key** gombra és kövesse az utasításokat
4. Ha sikeres volt a generálás, az **API Keys** menüpontban megjelenik az API KEy és az API Secret, az oldal tetején pedig láthatja a Cloud name-et is

### 3. Beállítások hozzáadása az alkalmazáshoz

1. Nyissa meg az `LessonLink/LessonLink.WebApi/appsettings.json` fájlt
2. A `CloudinarySettings` szekcióban írja be az oldalról kimásolt értékeket:

```json
"CloudinarySettings": {
  "CloudName": "az_on_cloud_name",
  "ApiKey": "123456789012345",
  "ApiSecret": "abcdefghijklmnopqrstuvwxyz123456"
}
```

4. Mentse a fájlt és indítsa újra a backend-et

**Megjegyzés:** A Cloudinary ingyenes csomagja 25 GB tárhely és 25 GB havi sávszélesség korlátozással rendelkezik, ami tesztelésre bőven elegendő.

# Technisch Ontwerp

## 1. Architectuur

### Frontend (Mobile)

* Framework: React Native
* Taal: TypeScript
* Architectuur: Component-based, scheiding tussen UI, business logic en state
* Structuur:

  * **components**: herbruikbare UI componenten
  * **views**: pagina’s
  * **services**: API communicatie
  * **hooks**: business logic
  * **store**: globale state
  * **types**: TypeScript definities

### Backend (API)

* Platform: .NET + Azure
* API: REST
* Architectuur: modulair en stateless

### Backoffice (Web)

* Framework: ASP.NET
* Architectuur: MVC (Model-View-Controller)

---

## 2. Ontwikkelomgeving

* **Node.js** ?
* **.NET SDK** 10.0.0
* IDE:

  * Visual Studio Code (frontend)
  * Visual Studio of VS Code (backend)
* Package managers:

  * npm

---

## 3. Afhankelijkheden

### Frontend

* React Native
* State management (Zustand of Redux)
* fetch voor API calls

### Backend

* ASP.NET Core
* Entity Framework (database)
* Azure services (SignalR)

---

## 4. Real-time Communicatie

* Gebruik van WebSockets via Azure SignalR
* Toepassingen:

  * live polls
  * Q&A tijdens sessies
  * updates van sessiecapaciteit
  * real-time dashboards
* Fallback: polling indien WebSockets niet beschikbaar

---

## 5. Coding Guidelines per Project

### 5.1 Frontend (React Native App)

* **Taal**: TypeScript verplicht, `any` vermijden
* **Bestandsnamen**:

  * componenten: PascalCase (`SessionCard.tsx`)
  * overige bestanden: camelCase (`apiService.ts`, `useAuth.ts`)
* **Naamgeving**:

  * variabelen & functies: camelCase
  * componenten & classes: PascalCase
* **Structuur & Herbruikbaarheid**:

  * componenten maximaal ±300 regels waar mogelijk
  * herbruikbare logica in hooks of services abstraheren
* **State Management**:

  * globale state in store, lokaal state in component
* **API Calls**:

  * via centrale service layer
  * async/await met error handling
* **Styling**:

  * voorkeur voor StyleSheet van React Native
  * herbruikbare styles in aparte modules

### 5.2 Backend (API – .NET)

* **Taal**: C# (laatste stabiele versie)
* **Bestandsnamen**:

  * controllers: PascalCase (`SessionController.cs`)
  * services: PascalCase (`PollService.cs`)
  * models/entities: PascalCase (`User.cs`)
* **Naamgeving**:

  * methods & properties: PascalCase
  * private fields: camelCase `_userRepository`
  * interfaces: prefix `I` (`IUserRepository`)
* **Structuur & Herbruikbaarheid**:

  * logica in services, controllers alleen voor request/response handling
  * dependency injection waar mogelijk
* **API Guidelines**:

  * standaard response structuur: `{ success: bool, data: object, error: string|null }`
  * HTTP status codes correct gebruiken
* **Security**:

  * JWT-authenticatie
  * input validatie en HTTPS verplicht
* **Database**:

  * Entity Framework Core
  * naming conventions: PascalCase voor tables/entities, camelCase voor properties

### 5.3 Backoffice (Web – ASP.NET)

* **Taal**: C# + Razor Pages / ASP.NET MVC
* **Bestandsnamen**:

  * controllers: PascalCase (`AdminController.cs`)
  * views: kebab-case (`session-overview.cshtml`)
  * scripts & styles: camelCase
* **Naamgeving**:

  * methods & properties: PascalCase
  * private fields: camelCase `_sessionService`
* **Structuur & Herbruikbaarheid**:

  * views enkel voor presentatie, logica in controllers/services
  * herbruikbare UI-componenten in partial views of Razor components
* **Styling**:

  * voorkeur voor Bootstrap classes
  * eigen CSS alleen in global.css of specifiek per pagina als nodig

---

## 6. Performance

* Lazy loading waar mogelijk
* Minimaliseer re-renders (React memoization)
* FlatList voor grote lijsten (front-office)
* Backend en realtime updates <1 seconde zichtbaar

---

## 7. Security

* JWT-authenticatie
* Secure token storage
* Input validatie op front- en back-office
* HTTPS-only
* Rate limiting op API endpoints

---

## 8. Testing

* **Front-office**: Jest + React Native Testing Library
* **Back-office**: xUnit / NUnit

* Mock API responses bij tests

---

## 9. Versiebeheer

* Git met GitHub
* **Branch Structuur**:

  * main: productie
  * develop: integratie
  * feature branches: feature/naam
  * fix branches: fix/naam
  * documentation branches: doc/naam
* Pull requests verplicht: min. 1 review voor fixes, 2 voor features
* Commit messages duidelijk en beschrijvend


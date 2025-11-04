# Guida Integrazione Template HTML con NugoloFamily PWA

## 📋 Panoramica

Questa guida spiega come integrare il template HTML Bootstrap esistente con l'infrastruttura PWA (Progressive Web App) già configurata per NugoloFamily.

## ✅ Infrastruttura Già Configurata

### 1. Struttura File Statici
```
NugoloFamily.API/wwwroot/
├── css/           # CSS del template
├── js/            # JavaScript del template e PWA
├── img/           # Immagini e icone
├── fonts/         # Font personalizzati
├── vendor/        # Plugin e librerie di terze parti
├── manifest.json  # PWA Manifest (già configurato)
├── sw.js          # Service Worker (già implementato)
└── index.html     # Pagina principale (da sostituire con il template)
```

### 2. File PWA Già Creati

#### ✅ `manifest.json`
- Configurazione completa per installazione PWA
- Icone per tutte le dimensioni (72x72 fino a 512x512)
- Shortcuts per accesso rapido
- Share target per condivisione file

#### ✅ `sw.js` (Service Worker)
- Cache intelligente (Network First per API, Cache First per assets)
- Funzionalità offline
- Background sync per sincronizzazione dati
- Supporto notifiche push
- Gestione aggiornamenti automatica

#### ✅ `js/pwa-handler.js`
- Registrazione Service Worker
- Gestione installazione PWA
- Rilevamento stato online/offline
- Sistema notifiche toast
- Gestione aggiornamenti app

#### ✅ `js/api-client.js`
- Client completo per tutte le API REST
- Gestione autenticazione JWT
- Gestione errori e token scaduti
- Supporto offline
- Metodi per tutte le entità (Famiglie, Utenti, Assistenti, Conversazioni, Messaggi, Documenti)

#### ✅ `css/style.css`
- Stili base per PWA
- Stili per chat interface
- Stili per cards documenti e assistenti
- Supporto dark mode
- Utility classes

### 3. Middleware Configurato
Nel file `Program.cs`:
- ✅ `app.UseStaticFiles()` - Serve file statici da wwwroot
- ✅ `app.UseDefaultFiles()` - Serve index.html come default
- ✅ CORS configurato per sviluppo frontend

## 🚀 Passi per Integrare il Template

### Passo 1: Copiare i File del Template

```bash
# Dalla cartella del template, copia tutti i file nella cartella wwwroot
cp -r template/css/* NugoloFamily.API/wwwroot/css/
cp -r template/js/* NugoloFamily.API/wwwroot/js/
cp -r template/img/* NugoloFamily.API/wwwroot/img/
cp -r template/fonts/* NugoloFamily.API/wwwroot/fonts/
cp -r template/vendor/* NugoloFamily.API/wwwroot/vendor/
cp template/*.html NugoloFamily.API/wwwroot/
```

### Passo 2: Modificare il File HTML Principale

Apri il file `index.html` del template e aggiungi le seguenti sezioni:

#### A. Nel `<head>`, dopo i meta tag esistenti:

```html
<!-- PWA Meta Tags -->
<link rel="manifest" href="/manifest.json">
<meta name="mobile-web-app-capable" content="yes">
<meta name="apple-mobile-web-app-capable" content="yes">
<meta name="apple-mobile-web-app-status-bar-style" content="black-translucent">
<meta name="apple-mobile-web-app-title" content="NugoloFamily">
<meta name="theme-color" content="#1e40af">

<!-- PWA Icons -->
<link rel="icon" type="image/png" sizes="192x192" href="/img/icon-192x192.png">
<link rel="apple-touch-icon" sizes="192x192" href="/img/icon-192x192.png">
```

#### B. Prima del tag di chiusura `</body>`:

```html
<!-- Toast Container per notifiche -->
<div id="toast-container" class="toast-container position-fixed top-0 end-0 p-3" style="z-index: 9999;"></div>

<!-- Pulsante installazione PWA (opzionale, può essere integrato nel template) -->
<button id="install-button" class="btn btn-primary" style="display: none;">
    <i class="bi bi-download me-2"></i>
    Installa App
</button>

<!-- API Client -->
<script src="/js/api-client.js"></script>

<!-- PWA Handler -->
<script src="/js/pwa-handler.js"></script>
```

### Passo 3: Creare le Icone PWA

Devi creare le icone dell'app nelle seguenti dimensioni e salvarle in `wwwroot/img/`:
- icon-72x72.png
- icon-96x96.png
- icon-128x128.png
- icon-144x144.png
- icon-152x152.png
- icon-192x192.png
- icon-384x384.png
- icon-512x512.png

**Consiglio**: Usa un tool online come https://realfavicongenerator.net/ o https://www.pwabuilder.com/imageGenerator

### Passo 4: Aggiornare il Manifest (opzionale)

Se vuoi personalizzare ulteriormente il manifest, modifica `wwwroot/manifest.json`:

```json
{
  "name": "NugoloFamily - Il Nome Completo della Tua App",
  "short_name": "NugoloFamily",
  "theme_color": "#TUO_COLORE_PRINCIPALE",
  "background_color": "#TUO_COLORE_SFONDO"
}
```

### Passo 5: Integrare le Chiamate API

In ogni pagina dove serve interagire con il backend, usa il client API globale `apiClient`:

#### Esempio: Login

```javascript
// In login.html o nel tuo file JS
async function handleLogin(event) {
    event.preventDefault();

    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    try {
        const response = await apiClient.login(username, password);

        // Salva i dati utente
        console.log('Login successful:', response);

        // Reindirizza alla dashboard
        window.location.href = '/dashboard.html';

        // Mostra notifica
        showToast('Login effettuato con successo!', 'success');

    } catch (error) {
        console.error('Login error:', error);
        showToast(`Errore: ${error.message}`, 'danger');
    }
}
```

#### Esempio: Caricare Lista Assistenti

```javascript
async function loadAssistenti() {
    const familyId = localStorage.getItem('familyId');

    try {
        const assistenti = await apiClient.getAssistentiFamiglia(familyId);

        // Renderizza gli assistenti nell'HTML
        const container = document.getElementById('assistenti-container');
        container.innerHTML = assistenti.map(a => `
            <div class="assistant-card ${a.Attivo ? 'active' : ''}">
                <div class="assistant-icon ${a.CodiceAssistente.toLowerCase()}">
                    <i class="bi ${getAssistantIcon(a.CodiceAssistente)}"></i>
                </div>
                <h5>${a.NomeAssistente}</h5>
                <p>${a.Descrizione}</p>
                <button onclick="toggleAssistente(${a.IdAssistente})"
                        class="btn btn-sm ${a.Attivo ? 'btn-danger' : 'btn-primary'}">
                    ${a.Attivo ? 'Disattiva' : 'Attiva'}
                </button>
            </div>
        `).join('');

    } catch (error) {
        console.error('Error loading assistenti:', error);
        showToast('Errore nel caricamento degli assistenti', 'danger');
    }
}
```

#### Esempio: Inviare Messaggio in Chat

```javascript
async function sendMessage(conversazioneId, testo) {
    try {
        const messaggio = await apiClient.inviaMessaggio({
            IdConversazione: conversazioneId,
            TestoMessaggio: testo,
            TipoContenuto: 'Testo',
            Mittente: 'Utente'
        });

        // Aggiungi il messaggio alla UI
        appendMessageToChat(messaggio);

        // Il messaggio dell'assistente arriverà come risposta
        // o tramite polling/websocket (da implementare)

    } catch (error) {
        console.error('Error sending message:', error);
        showToast('Errore nell\'invio del messaggio', 'danger');
    }
}
```

### Passo 6: Gestire l'Autenticazione

#### Proteggere le Pagine

All'inizio di ogni pagina protetta, aggiungi:

```javascript
// Verifica autenticazione all'inizio della pagina
document.addEventListener('DOMContentLoaded', () => {
    if (!apiClient.isAuthenticated()) {
        // Reindirizza al login se non autenticato
        window.location.href = '/login.html';
        return;
    }

    // Carica i dati della pagina
    loadPageData();
});
```

#### Logout

```javascript
function logout() {
    // Pulisce tutti i dati di autenticazione e reindirizza
    apiClient.logout();
}
```

### Passo 7: Aggiornare il Service Worker

Se aggiungi nuovi file statici importanti che devono essere disponibili offline, aggiorna l'array `STATIC_ASSETS` in `wwwroot/sw.js`:

```javascript
const STATIC_ASSETS = [
  '/',
  '/index.html',
  '/dashboard.html',  // Aggiungi le tue pagine
  '/login.html',
  '/css/bootstrap.min.css',
  '/css/bootstrap-icons.css',
  '/css/style.css',
  '/css/TUO_CUSTOM_CSS.css',  // I tuoi CSS
  '/js/bootstrap.bundle.min.js',
  '/js/app.js',
  '/js/TUO_CUSTOM_JS.js',  // I tuoi JS
  '/manifest.json',
  '/img/logo.png'
];
```

## 📱 Funzionalità PWA Disponibili

### Installazione App

Il pulsante di installazione appare automaticamente sui dispositivi supportati. Puoi personalizzare dove posizionarlo nel tuo template.

### Notifiche

Usa la funzione `showToast()` per mostrare notifiche:

```javascript
showToast('Messaggio di successo', 'success');
showToast('Attenzione!', 'warning');
showToast('Errore critico', 'danger');
showToast('Informazione', 'info');
```

### Rilevamento Offline

Il sistema rileva automaticamente quando l'app va offline e mostra una notifica. Le API falliranno gracefully e tenteranno di usare la cache.

### Background Sync

Quando la connessione torna online, il service worker sincronizza automaticamente i dati pendenti (se implementato).

## 🎨 Personalizzazione Stili

Il file `wwwroot/css/style.css` contiene variabili CSS che puoi personalizzare:

```css
:root {
  --primary-color: #1e40af;      /* Colore principale */
  --secondary-color: #667eea;    /* Colore secondario */
  --accent-color: #764ba2;       /* Colore accento */
  /* ... altre variabili ... */
}
```

## 🔧 Configurazione Multi-Tenant

Il sistema multi-tenant è già implementato nel backend. Nel frontend:

1. Dopo il login, l'`IdFamiglia` viene salvato in `localStorage`
2. Tutte le chiamate API includono automaticamente il token JWT
3. Il backend filtra automaticamente i dati per famiglia

```javascript
// Recupera sempre l'ID famiglia corrente
const familyId = localStorage.getItem('familyId');

// Usalo nelle chiamate API
const conversazioni = await apiClient.getConversazioni(familyId);
```

## 📊 Esempi di Pagine da Creare

### Dashboard
- Mostra assistenti attivi
- Conversazioni recenti
- Documenti recenti
- Statistiche famiglia

### Chat
- Lista conversazioni per assistente
- Interface di chat con messaggi
- Upload documenti/immagini
- Registrazione vocale

### Documenti
- Lista documenti con filtri
- Upload nuovi documenti
- Anteprima e download
- Categorizzazione

### Impostazioni
- Gestione utenti famiglia
- Attivazione/disattivazione assistenti
- Configurazione AI provider
- Cambio password

## 🐛 Debug e Testing

### Testare la PWA

1. **Chrome DevTools**:
   - Apri DevTools (F12)
   - Tab "Application"
   - Sezione "Service Workers" per vedere lo stato del SW
   - Sezione "Manifest" per verificare il manifest
   - Sezione "Cache Storage" per vedere cosa è in cache

2. **Lighthouse**:
   - DevTools > Lighthouse
   - Seleziona "Progressive Web App"
   - Genera report

3. **Test Offline**:
   - DevTools > Network
   - Seleziona "Offline" dal dropdown
   - Testa la navigazione

### Console Logging

Il sistema logga automaticamente in console:
- Registrazione Service Worker
- Stato autenticazione
- Chiamate API
- Eventi PWA

## ⚠️ Note Importanti

1. **HTTPS Richiesto**: Le PWA funzionano solo su HTTPS (o localhost per sviluppo)

2. **Icone Necessarie**: Crea tutte le icone richieste per una buona esperienza su tutti i dispositivi

3. **Cache Strategy**: Attualmente usa "Network First" per API e "Cache First" per assets statici. Puoi modificare in `sw.js`

4. **API Base URL**: Di default è `/api`. Se il backend è su un dominio diverso, modifica in `api-client.js`:
   ```javascript
   const apiClient = new NugoloFamilyAPI('https://api.tuodominio.com/api');
   ```

5. **Versioning**: Quando aggiorni il SW, cambia il `CACHE_NAME` in `sw.js` per forzare il refresh della cache:
   ```javascript
   const CACHE_NAME = 'nugolofamily-v2'; // incrementa versione
   ```

## 📚 Risorse Utili

- [MDN: Progressive Web Apps](https://developer.mozilla.org/en-US/docs/Web/Progressive_web_apps)
- [web.dev: PWA](https://web.dev/progressive-web-apps/)
- [PWA Builder](https://www.pwabuilder.com/)
- [Workbox (Google PWA library)](https://developers.google.com/web/tools/workbox)

## 🎯 Prossimi Passi

1. ✅ Copia i file del template in `wwwroot/`
2. ✅ Aggiungi i tag PWA al template HTML
3. ✅ Crea le icone PWA
4. ✅ Integra le chiamate API nelle pagine
5. ⏳ Testa l'installazione PWA
6. ⏳ Testa funzionalità offline
7. ⏳ Implementa le pagine principali (Dashboard, Chat, Documenti, Impostazioni)
8. ⏳ Personalizza stili e colori
9. ⏳ Testa su dispositivi mobili

---

**Buon lavoro! Se hai domande o problemi durante l'integrazione, controlla prima i log della console del browser per messaggi di errore dettagliati.**

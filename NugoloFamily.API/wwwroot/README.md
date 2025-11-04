# NugoloFamily PWA - Frontend Assets

## 📁 Struttura Cartelle

```
wwwroot/
├── css/           # Fogli di stile (Bootstrap + custom)
├── js/            # JavaScript (Bootstrap + PWA + API Client)
├── img/           # Immagini, icone PWA, e assets grafici
├── fonts/         # Font personalizzati
├── vendor/        # Plugin e librerie di terze parti
├── manifest.json  # PWA Manifest per installazione app
├── sw.js          # Service Worker per funzionalità offline e cache
└── index.html     # Pagina principale (da sostituire con il template)
```

## 🚀 File Implementati

### PWA Core Files

1. **manifest.json**
   - Configurazione PWA completa
   - Definisce nome, icone, colori dell'app
   - Shortcuts per accesso rapido
   - Share target per condivisione file

2. **sw.js** (Service Worker)
   - Gestione cache intelligente
   - Funzionalità offline
   - Background sync
   - Push notifications support
   - Auto-update handling

3. **js/pwa-handler.js**
   - Registrazione Service Worker
   - Gestione installazione PWA
   - Rilevamento online/offline
   - Sistema notifiche toast
   - Gestione aggiornamenti app

4. **js/api-client.js**
   - Client completo per API REST
   - Autenticazione JWT
   - Gestione errori
   - Supporto offline
   - Metodi per tutte le entità del backend

5. **css/style.css**
   - Stili PWA
   - Chat interface
   - Document & assistant cards
   - Dark mode support
   - Responsive utilities
   - Accessibility features

## 📋 File da Aggiungere (dal Template)

Quando integri il template HTML, aggiungi:

### CSS
- Bootstrap theme files
- Custom template styles
- Plugin-specific stylesheets

### JavaScript
- Template main JS
- Plugin scripts (Tiny Slider, Range Slider, etc.)
- Custom application logic

### Images
- **Icone PWA** (IMPORTANTE!):
  - icon-72x72.png
  - icon-96x96.png
  - icon-128x128.png
  - icon-144x144.png
  - icon-152x152.png
  - icon-192x192.png
  - icon-384x384.png
  - icon-512x512.png
- Logo e branding
- Background images
- UI elements

### Fonts
- Google Fonts (se scaricati localmente)
- Custom fonts del template

### Vendor
- Bootstrap (se non CDN)
- Bootstrap Icons
- Plugin:
  - Counter Up
  - Range Slider
  - Countdown
  - Password Meter
  - Tiny Slider
  - Slide Toggle
  - Venobox
  - Data Table
  - Apex Charts
  - Google Maps (configurazione)

## 🔧 Come Usare

### 1. Copiare il Template

```bash
# Dalla cartella del template
cp -r css/* wwwroot/css/
cp -r js/* wwwroot/js/
cp -r img/* wwwroot/img/
cp -r fonts/* wwwroot/fonts/
cp -r vendor/* wwwroot/vendor/
cp *.html wwwroot/
```

### 2. Integrare PWA nel Template

Vedi `GUIDA_INTEGRAZIONE_TEMPLATE.md` nella root del progetto per istruzioni dettagliate.

### 3. Testare

```bash
# Avvia il server
cd NugoloFamily.API
dotnet run

# Apri browser
# https://localhost:5001
```

## 🎯 API Client - Metodi Disponibili

Il file `js/api-client.js` espone un'istanza globale `apiClient` con i seguenti metodi:

### Autenticazione
- `apiClient.login(username, password)`
- `apiClient.register(userData)`
- `apiClient.changePassword(idUtente, passwordCorrente, nuovaPassword)`
- `apiClient.logout()`
- `apiClient.isAuthenticated()`

### Famiglie
- `apiClient.getFamiglia(idFamiglia)`
- `apiClient.createFamiglia(famigliaData)`
- `apiClient.updateFamiglia(idFamiglia, famigliaData)`

### Utenti
- `apiClient.getUtentiFamiglia(idFamiglia)`
- `apiClient.getUtente(idUtente)`

### Assistenti
- `apiClient.getAssistenti()`
- `apiClient.getAssistentiFamiglia(idFamiglia)`
- `apiClient.attivaAssistente(idFamiglia, idAssistente)`
- `apiClient.disattivaAssistente(idFamiglia, idAssistente)`
- `apiClient.configuraAssistente(idFamiglia, idAssistente, config)`

### Conversazioni
- `apiClient.getConversazioni(idFamiglia)`
- `apiClient.getConversazione(idConversazione)`
- `apiClient.getConversazioniAssistente(idFamiglia, idAssistente)`
- `apiClient.createConversazione(conversazioneData)`
- `apiClient.updateConversazione(idConversazione, conversazioneData)`

### Messaggi
- `apiClient.getMessaggiConversazione(idConversazione)`
- `apiClient.inviaMessaggio(messaggioData)`
- `apiClient.inviaMessaggioVocale(idConversazione, audioBlob)`

### Documenti
- `apiClient.getDocumenti(idFamiglia)`
- `apiClient.getDocumento(idDocumento)`
- `apiClient.getDocumentoDettaglio(idDocumento)`
- `apiClient.getDocumentiPerTipo(idFamiglia, tipoDocumento)`
- `apiClient.getDocumentiPerCategoria(idFamiglia, categoria)`
- `apiClient.uploadDocumento(documentoData)`
- `apiClient.deleteDocumento(idDocumento)`
- `apiClient.elaboraDocumento(idDocumento)`

## 📱 PWA Features

### Installazione
L'app può essere installata su desktop e mobile come app nativa. Il pulsante appare automaticamente.

### Offline
L'app funziona offline con cache intelligente:
- **API**: Network First (con fallback su cache)
- **Assets**: Cache First (con fallback su network)

### Notifiche
Usa `showToast(message, type)` per mostrare notifiche:
```javascript
showToast('Operazione completata!', 'success');
showToast('Attenzione!', 'warning');
showToast('Errore!', 'danger');
showToast('Info', 'info');
```

### Background Sync
Quando torna la connessione, i dati pendenti vengono sincronizzati automaticamente.

## 🔐 Autenticazione

Dopo il login, i seguenti dati sono salvati in localStorage:
- `authToken`: Token JWT
- `familyId`: ID della famiglia
- `userId`: ID dell'utente
- `username`: Username

Tutte le chiamate API successive includeranno automaticamente il token.

## 🎨 Personalizzazione

Modifica le variabili CSS in `css/style.css`:

```css
:root {
  --primary-color: #1e40af;
  --secondary-color: #667eea;
  --accent-color: #764ba2;
  /* ... */
}
```

## 📚 Documentazione

- Guida completa: `/GUIDA_INTEGRAZIONE_TEMPLATE.md`
- API Documentation: `https://localhost:5001/swagger`
- Service Worker Console: Browser DevTools > Application > Service Workers

## ⚠️ Note Importanti

1. Le PWA richiedono **HTTPS** (localhost è ok per sviluppo)
2. Crea tutte le **icone PWA** richieste
3. Aggiorna il `CACHE_NAME` in `sw.js` ad ogni modifica importante
4. Testa sempre in modalità **offline** prima del deploy

## 🐛 Debug

Apri Chrome DevTools (F12):
- **Console**: Log di eventi PWA e API
- **Application > Service Workers**: Stato del SW
- **Application > Manifest**: Validazione manifest
- **Application > Cache Storage**: Contenuto cache
- **Network**: Simula offline mode
- **Lighthouse**: Audit PWA completo

// PWA Handler - Gestisce l'installazione e il service worker

let deferredPrompt;
let isInstalled = false;

// Registra il Service Worker
if ('serviceWorker' in navigator) {
  window.addEventListener('load', () => {
    navigator.serviceWorker.register('/sw.js')
      .then((registration) => {
        console.log('[PWA] Service Worker registrato con successo:', registration.scope);

        // Controlla aggiornamenti periodicamente
        setInterval(() => {
          registration.update();
        }, 60000); // Ogni minuto

        // Gestisci gli aggiornamenti del Service Worker
        registration.addEventListener('updatefound', () => {
          const newWorker = registration.installing;
          console.log('[PWA] Nuovo Service Worker trovato, installazione in corso...');

          newWorker.addEventListener('statechange', () => {
            if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
              // Nuovo service worker disponibile
              showUpdateNotification();
            }
          });
        });
      })
      .catch((error) => {
        console.error('[PWA] Errore registrazione Service Worker:', error);
      });

    // Gestisci il controller change (quando un nuovo SW prende il controllo)
    navigator.serviceWorker.addEventListener('controllerchange', () => {
      console.log('[PWA] Nuovo Service Worker attivo, ricarica la pagina');
      window.location.reload();
    });
  });
}

// Intercetta l'evento beforeinstallprompt
window.addEventListener('beforeinstallprompt', (e) => {
  console.log('[PWA] beforeinstallprompt event triggered');

  // Previeni il prompt automatico di Chrome
  e.preventDefault();

  // Salva l'evento per utilizzarlo successivamente
  deferredPrompt = e;

  // Mostra il pulsante di installazione personalizzato
  showInstallButton();
});

// Gestisci l'evento appinstalled
window.addEventListener('appinstalled', () => {
  console.log('[PWA] App installata con successo!');
  isInstalled = true;

  // Nascondi il pulsante di installazione
  hideInstallButton();

  // Mostra messaggio di conferma
  showToast('NugoloFamily installato con successo!', 'success');

  // Log analytics (opzionale)
  if (window.gtag) {
    gtag('event', 'pwa_installed', {
      event_category: 'PWA',
      event_label: 'App Installed'
    });
  }
});

// Funzione per mostrare il pulsante di installazione
function showInstallButton() {
  const installButton = document.getElementById('install-button');
  if (installButton) {
    installButton.style.display = 'block';
    installButton.addEventListener('click', installPWA);
  }
}

// Funzione per nascondere il pulsante di installazione
function hideInstallButton() {
  const installButton = document.getElementById('install-button');
  if (installButton) {
    installButton.style.display = 'none';
  }
}

// Funzione per installare la PWA
async function installPWA() {
  if (!deferredPrompt) {
    console.log('[PWA] Prompt di installazione non disponibile');
    return;
  }

  // Mostra il prompt di installazione
  deferredPrompt.prompt();

  // Attendi la scelta dell'utente
  const { outcome } = await deferredPrompt.userChoice;
  console.log(`[PWA] Scelta utente: ${outcome}`);

  // Log analytics
  if (window.gtag) {
    gtag('event', 'pwa_install_prompt', {
      event_category: 'PWA',
      event_label: outcome
    });
  }

  // Reset del prompt
  deferredPrompt = null;
  hideInstallButton();
}

// Verifica se l'app è già installata
function checkIfInstalled() {
  // Controlla se l'app è in modalità standalone (installata)
  if (window.matchMedia('(display-mode: standalone)').matches) {
    isInstalled = true;
    console.log('[PWA] App già installata (standalone mode)');
    return true;
  }

  // iOS Safari
  if (window.navigator.standalone === true) {
    isInstalled = true;
    console.log('[PWA] App già installata (iOS standalone)');
    return true;
  }

  return false;
}

// Mostra notifica di aggiornamento disponibile
function showUpdateNotification() {
  const updateBanner = document.createElement('div');
  updateBanner.className = 'update-banner';
  updateBanner.innerHTML = `
    <div class="alert alert-info alert-dismissible fade show" role="alert">
      <strong>Aggiornamento disponibile!</strong>
      Una nuova versione di NugoloFamily è pronta.
      <button type="button" class="btn btn-sm btn-primary ms-2" onclick="refreshApp()">
        Aggiorna Ora
      </button>
      <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    </div>
  `;

  document.body.insertBefore(updateBanner, document.body.firstChild);
}

// Ricarica l'app per applicare l'aggiornamento
function refreshApp() {
  navigator.serviceWorker.getRegistration().then((registration) => {
    if (registration && registration.waiting) {
      // Invia messaggio al service worker in attesa per attivarlo
      registration.waiting.postMessage({ type: 'SKIP_WAITING' });
    }
  });
}

// Gestione dello stato offline/online
window.addEventListener('online', () => {
  console.log('[PWA] Connessione ripristinata');
  showToast('Connessione ripristinata', 'success');

  // Trigger background sync se disponibile
  if ('serviceWorker' in navigator && 'sync' in ServiceWorkerRegistration.prototype) {
    navigator.serviceWorker.ready.then((registration) => {
      registration.sync.register('sync-messages');
      registration.sync.register('sync-documents');
    });
  }
});

window.addEventListener('offline', () => {
  console.log('[PWA] Modalità offline attiva');
  showToast('Modalità offline attiva - Alcune funzionalità potrebbero essere limitate', 'warning');
});

// Utility: mostra un toast/notifica
function showToast(message, type = 'info') {
  // Implementa la tua logica di toast notification
  // Puoi usare Bootstrap Toast, Toastr, o una soluzione custom
  console.log(`[Toast ${type}] ${message}`);

  // Esempio con Bootstrap Toast (se disponibile)
  const toastContainer = document.getElementById('toast-container');
  if (toastContainer) {
    const toastHTML = `
      <div class="toast align-items-center text-white bg-${type === 'success' ? 'success' : type === 'warning' ? 'warning' : 'info'} border-0" role="alert" aria-live="assertive" aria-atomic="true">
        <div class="d-flex">
          <div class="toast-body">
            ${message}
          </div>
          <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
      </div>
    `;

    toastContainer.insertAdjacentHTML('beforeend', toastHTML);

    const toastElement = toastContainer.lastElementChild;
    const toast = new bootstrap.Toast(toastElement, { delay: 3000 });
    toast.show();

    // Rimuovi il toast dopo che è stato nascosto
    toastElement.addEventListener('hidden.bs.toast', () => {
      toastElement.remove();
    });
  }
}

// Inizializza al caricamento
document.addEventListener('DOMContentLoaded', () => {
  checkIfInstalled();

  // Crea container per i toast se non esiste
  if (!document.getElementById('toast-container')) {
    const container = document.createElement('div');
    container.id = 'toast-container';
    container.className = 'toast-container position-fixed top-0 end-0 p-3';
    container.style.zIndex = '9999';
    document.body.appendChild(container);
  }
});

// Esponi funzioni globali
window.installPWA = installPWA;
window.refreshApp = refreshApp;
window.showToast = showToast;

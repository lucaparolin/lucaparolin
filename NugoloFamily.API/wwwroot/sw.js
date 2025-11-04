// Service Worker per NugoloFamily PWA
const CACHE_NAME = 'nugolofamily-v1';
const API_CACHE_NAME = 'nugolofamily-api-v1';

// File da cachare per funzionamento offline
const STATIC_ASSETS = [
  '/',
  '/index.html',
  '/css/bootstrap.min.css',
  '/css/bootstrap-icons.css',
  '/css/style.css',
  '/js/bootstrap.bundle.min.js',
  '/js/app.js',
  '/manifest.json',
  '/img/logo.png'
];

// Installazione del Service Worker
self.addEventListener('install', (event) => {
  console.log('[Service Worker] Installing...');
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then((cache) => {
        console.log('[Service Worker] Caching static assets');
        return cache.addAll(STATIC_ASSETS);
      })
      .catch((error) => {
        console.error('[Service Worker] Cache failed:', error);
      })
  );
  self.skipWaiting();
});

// Attivazione del Service Worker
self.addEventListener('activate', (event) => {
  console.log('[Service Worker] Activating...');
  event.waitUntil(
    caches.keys().then((cacheNames) => {
      return Promise.all(
        cacheNames.map((cacheName) => {
          if (cacheName !== CACHE_NAME && cacheName !== API_CACHE_NAME) {
            console.log('[Service Worker] Deleting old cache:', cacheName);
            return caches.delete(cacheName);
          }
        })
      );
    })
  );
  return self.clients.claim();
});

// Strategia di caching: Network First per API, Cache First per assets statici
self.addEventListener('fetch', (event) => {
  const { request } = event;
  const url = new URL(request.url);

  // API requests - Network First (con fallback su cache)
  if (url.pathname.startsWith('/api/')) {
    event.respondWith(
      fetch(request)
        .then((response) => {
          // Clona la risposta prima di metterla in cache
          const responseToCache = response.clone();

          // Cache solo le risposte GET successful
          if (request.method === 'GET' && response.status === 200) {
            caches.open(API_CACHE_NAME).then((cache) => {
              cache.put(request, responseToCache);
            });
          }

          return response;
        })
        .catch(() => {
          // Se la rete fallisce, prova a recuperare dalla cache
          return caches.match(request).then((cachedResponse) => {
            if (cachedResponse) {
              console.log('[Service Worker] Serving from cache (offline):', request.url);
              return cachedResponse;
            }

            // Se non c'è nemmeno in cache, restituisci una risposta di errore
            return new Response(
              JSON.stringify({
                error: 'Offline - Risorsa non disponibile in cache',
                offline: true
              }),
              {
                status: 503,
                headers: { 'Content-Type': 'application/json' }
              }
            );
          });
        })
    );
    return;
  }

  // Static assets - Cache First (con fallback su network)
  event.respondWith(
    caches.match(request)
      .then((cachedResponse) => {
        if (cachedResponse) {
          console.log('[Service Worker] Serving from cache:', request.url);
          return cachedResponse;
        }

        // Se non è in cache, scarica dalla rete
        return fetch(request).then((response) => {
          // Non cachare risposte non-successful
          if (!response || response.status !== 200 || response.type === 'error') {
            return response;
          }

          // Clona la risposta
          const responseToCache = response.clone();

          // Aggiungi alla cache
          caches.open(CACHE_NAME).then((cache) => {
            cache.put(request, responseToCache);
          });

          return response;
        });
      })
      .catch((error) => {
        console.error('[Service Worker] Fetch failed:', error);

        // Fallback per le pagine HTML
        if (request.headers.get('Accept').includes('text/html')) {
          return caches.match('/index.html');
        }

        return new Response('Offline - Risorsa non disponibile', {
          status: 503,
          statusText: 'Service Unavailable'
        });
      })
  );
});

// Background Sync per sincronizzare i dati quando torna la connessione
self.addEventListener('sync', (event) => {
  console.log('[Service Worker] Background sync:', event.tag);

  if (event.tag === 'sync-messages') {
    event.waitUntil(syncMessages());
  }

  if (event.tag === 'sync-documents') {
    event.waitUntil(syncDocuments());
  }
});

// Notifiche Push
self.addEventListener('push', (event) => {
  console.log('[Service Worker] Push received:', event);

  const options = {
    body: event.data ? event.data.text() : 'Nuova notifica da NugoloFamily',
    icon: '/img/icon-192x192.png',
    badge: '/img/badge-72x72.png',
    vibrate: [200, 100, 200],
    data: {
      dateOfArrival: Date.now(),
      primaryKey: 1
    },
    actions: [
      {
        action: 'explore',
        title: 'Apri',
        icon: '/img/checkmark.png'
      },
      {
        action: 'close',
        title: 'Chiudi',
        icon: '/img/xmark.png'
      }
    ]
  };

  event.waitUntil(
    self.registration.showNotification('NugoloFamily', options)
  );
});

// Gestione click sulle notifiche
self.addEventListener('notificationclick', (event) => {
  console.log('[Service Worker] Notification click:', event.action);

  event.notification.close();

  if (event.action === 'explore') {
    event.waitUntil(
      clients.openWindow('/')
    );
  }
});

// Funzioni helper per la sincronizzazione
async function syncMessages() {
  try {
    // Qui implementerai la logica per sincronizzare i messaggi pendenti
    console.log('[Service Worker] Syncing messages...');

    // Recupera i messaggi pendenti da IndexedDB
    // Invia al server
    // Marca come sincronizzati

    return Promise.resolve();
  } catch (error) {
    console.error('[Service Worker] Sync messages failed:', error);
    return Promise.reject(error);
  }
}

async function syncDocuments() {
  try {
    // Qui implementerai la logica per sincronizzare i documenti pendenti
    console.log('[Service Worker] Syncing documents...');

    return Promise.resolve();
  } catch (error) {
    console.error('[Service Worker] Sync documents failed:', error);
    return Promise.reject(error);
  }
}

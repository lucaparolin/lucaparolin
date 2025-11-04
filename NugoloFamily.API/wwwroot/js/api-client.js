// API Client per NugoloFamily
// Gestisce tutte le chiamate HTTP al backend REST API

class NugoloFamilyAPI {
  constructor(baseUrl = '/api') {
    this.baseUrl = baseUrl;
    this.token = localStorage.getItem('authToken');
    this.familyId = localStorage.getItem('familyId');
  }

  // Imposta il token JWT
  setToken(token) {
    this.token = token;
    localStorage.setItem('authToken', token);
  }

  // Imposta l'ID famiglia
  setFamilyId(familyId) {
    this.familyId = familyId;
    localStorage.setItem('familyId', familyId);
  }

  // Rimuove token e dati di autenticazione
  clearAuth() {
    this.token = null;
    this.familyId = null;
    localStorage.removeItem('authToken');
    localStorage.removeItem('familyId');
    localStorage.removeItem('userId');
    localStorage.removeItem('username');
  }

  // Verifica se l'utente è autenticato
  isAuthenticated() {
    return !!this.token;
  }

  // Headers comuni per le richieste
  getHeaders(contentType = 'application/json') {
    const headers = {
      'Content-Type': contentType
    };

    if (this.token) {
      headers['Authorization'] = `Bearer ${this.token}`;
    }

    return headers;
  }

  // Gestione errori HTTP
  async handleResponse(response) {
    // Controlla se il token è scaduto
    if (response.headers.get('Token-Expired') === 'true') {
      this.clearAuth();
      window.location.href = '/login.html';
      throw new Error('Token scaduto. Effettua nuovamente il login.');
    }

    const contentType = response.headers.get('content-type');
    const isJson = contentType && contentType.includes('application/json');

    if (!response.ok) {
      const error = isJson ? await response.json() : await response.text();
      throw new Error(error.Message || error || `HTTP ${response.status}: ${response.statusText}`);
    }

    return isJson ? await response.json() : await response.text();
  }

  // Metodo generico per richieste HTTP
  async request(endpoint, options = {}) {
    const url = `${this.baseUrl}${endpoint}`;

    try {
      const response = await fetch(url, {
        ...options,
        headers: this.getHeaders(options.contentType)
      });

      return await this.handleResponse(response);
    } catch (error) {
      console.error(`[API] Errore chiamata ${endpoint}:`, error);

      // Mostra notifica se l'app è offline
      if (!navigator.onLine) {
        showToast('Modalità offline - Operazione non disponibile', 'warning');
      }

      throw error;
    }
  }

  // ======================
  // AUTENTICAZIONE
  // ======================

  async login(username, password) {
    const data = await this.request('/utenti/login', {
      method: 'POST',
      body: JSON.stringify({ Username: username, Password: password })
    });

    if (data.Token) {
      this.setToken(data.Token);
      this.setFamilyId(data.Utente.IdFamiglia);
      localStorage.setItem('userId', data.Utente.IdUtente);
      localStorage.setItem('username', data.Utente.Username);
    }

    return data;
  }

  async register(userData) {
    return await this.request('/utenti/register', {
      method: 'POST',
      body: JSON.stringify(userData)
    });
  }

  async changePassword(idUtente, passwordCorrente, nuovaPassword) {
    return await this.request('/utenti/cambia-password', {
      method: 'POST',
      body: JSON.stringify({
        IdUtente: idUtente,
        PasswordCorrente: passwordCorrente,
        NuovaPassword: nuovaPassword
      })
    });
  }

  logout() {
    this.clearAuth();
    window.location.href = '/login.html';
  }

  // ======================
  // FAMIGLIE
  // ======================

  async getFamiglia(idFamiglia) {
    return await this.request(`/famiglie/${idFamiglia}`);
  }

  async createFamiglia(famigliaData) {
    return await this.request('/famiglie', {
      method: 'POST',
      body: JSON.stringify(famigliaData)
    });
  }

  async updateFamiglia(idFamiglia, famigliaData) {
    return await this.request(`/famiglie/${idFamiglia}`, {
      method: 'PUT',
      body: JSON.stringify(famigliaData)
    });
  }

  // ======================
  // UTENTI
  // ======================

  async getUtentiFamiglia(idFamiglia) {
    return await this.request(`/utenti/famiglia/${idFamiglia}`);
  }

  async getUtente(idUtente) {
    return await this.request(`/utenti/${idUtente}`);
  }

  // ======================
  // ASSISTENTI
  // ======================

  async getAssistenti() {
    return await this.request('/assistenti');
  }

  async getAssistentiFamiglia(idFamiglia) {
    return await this.request(`/assistenti/famiglia/${idFamiglia}`);
  }

  async attivaAssistente(idFamiglia, idAssistente) {
    return await this.request(`/assistenti/attiva`, {
      method: 'POST',
      body: JSON.stringify({
        IdFamiglia: idFamiglia,
        IdAssistente: idAssistente
      })
    });
  }

  async disattivaAssistente(idFamiglia, idAssistente) {
    return await this.request(`/assistenti/disattiva`, {
      method: 'POST',
      body: JSON.stringify({
        IdFamiglia: idFamiglia,
        IdAssistente: idAssistente
      })
    });
  }

  async configuraAssistente(idFamiglia, idAssistente, config) {
    return await this.request(`/assistenti/configura`, {
      method: 'POST',
      body: JSON.stringify({
        IdFamiglia: idFamiglia,
        IdAssistente: idAssistente,
        ...config
      })
    });
  }

  // ======================
  // CONVERSAZIONI
  // ======================

  async getConversazioni(idFamiglia) {
    return await this.request(`/conversazioni/famiglia/${idFamiglia}`);
  }

  async getConversazione(idConversazione) {
    return await this.request(`/conversazioni/${idConversazione}`);
  }

  async getConversazioniAssistente(idFamiglia, idAssistente) {
    return await this.request(`/conversazioni/famiglia/${idFamiglia}/assistente/${idAssistente}`);
  }

  async createConversazione(conversazioneData) {
    return await this.request('/conversazioni', {
      method: 'POST',
      body: JSON.stringify(conversazioneData)
    });
  }

  async updateConversazione(idConversazione, conversazioneData) {
    return await this.request(`/conversazioni/${idConversazione}`, {
      method: 'PUT',
      body: JSON.stringify(conversazioneData)
    });
  }

  // ======================
  // MESSAGGI
  // ======================

  async getMessaggiConversazione(idConversazione) {
    return await this.request(`/messaggi/conversazione/${idConversazione}`);
  }

  async inviaMessaggio(messaggioData) {
    return await this.request('/messaggi', {
      method: 'POST',
      body: JSON.stringify(messaggioData)
    });
  }

  async inviaMessaggioVocale(idConversazione, audioBlob) {
    const formData = new FormData();
    formData.append('IdConversazione', idConversazione);
    formData.append('audio', audioBlob, 'voice-message.webm');

    return await this.request('/messaggi/vocale', {
      method: 'POST',
      body: formData,
      contentType: null // Lascia che fetch imposti il content-type per FormData
    });
  }

  // ======================
  // DOCUMENTI
  // ======================

  async getDocumenti(idFamiglia) {
    return await this.request(`/documenti/famiglia/${idFamiglia}`);
  }

  async getDocumento(idDocumento) {
    return await this.request(`/documenti/${idDocumento}`);
  }

  async getDocumentoDettaglio(idDocumento) {
    return await this.request(`/documenti/${idDocumento}/dettaglio`);
  }

  async getDocumentiPerTipo(idFamiglia, tipoDocumento) {
    return await this.request(`/documenti/famiglia/${idFamiglia}/tipo/${encodeURIComponent(tipoDocumento)}`);
  }

  async getDocumentiPerCategoria(idFamiglia, categoria) {
    return await this.request(`/documenti/famiglia/${idFamiglia}/categoria/${encodeURIComponent(categoria)}`);
  }

  async uploadDocumento(documentoData) {
    return await this.request('/documenti/upload', {
      method: 'POST',
      body: JSON.stringify(documentoData)
    });
  }

  async deleteDocumento(idDocumento) {
    return await this.request(`/documenti/${idDocumento}`, {
      method: 'DELETE'
    });
  }

  async elaboraDocumento(idDocumento) {
    return await this.request(`/documenti/${idDocumento}/elabora`, {
      method: 'POST'
    });
  }

  // ======================
  // HEALTH CHECK
  // ======================

  async healthCheck() {
    return await this.request('/health', { baseUrl: '' });
  }
}

// Crea un'istanza globale dell'API client
const apiClient = new NugoloFamilyAPI();

// Esponi globalmente
window.apiClient = apiClient;
window.NugoloFamilyAPI = NugoloFamilyAPI;

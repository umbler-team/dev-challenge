const Request = window.Request
const Headers = window.Headers
const fetch = window.fetch

class ProgressBar {
    constructor() {
        this.totalSteps = 2;
        this.maximumPercentage = 100;
        this.progressBarStep = 100 / this.totalSteps;
        this.progressBarPercentage = 100 / this.totalSteps;
    }

    upProgressBar() {
        this.progressBarPercentage += this.progressBarStep;

        domManager.setProgressBarValue(this.progressBarPercentage);
    }

    startLoad() {
        domManager.showProgressBar();
    }

    stopLoad() {
        domManager.hideProgressBar();
    }
}

class DOMManager {
    constructor() {
        this.DOMValidateField = document.getElementById('validate-field');
        this.DOMInputSearchGroup = document.getElementById('input-group-search');
        this.DOMDomain = document.getElementById('txt-search');
        this.whoisResultDOM = document.getElementById('whois-results');
        this.progressBarDOM = document.getElementById('progress-request');
        this.nameShow = document.getElementById("name-show");
        this.ipShow = document.getElementById("ip-show");
        this.hostShow = document.getElementById("host-show");
        this.whoisShow = document.getElementById("whois-show");
        this.nsRecordsShow = document.getElementById("dns-form");
    }

    resetDOM() {
        this.removeWarning(this.DOMInputSearchGroup);
        this.setInvisible(this.DOMValidateField);
        this.setInvisible(this.whoisResultDOM);
        this.removeDnsList();
        this.hideProgressBar();
    }

    showProgressBar() {
        this.setVisible(this.progressBarDOM);
    }

    hideProgressBar() {
        this.setInvisible(this.progressBarDOM);
    }

    getProgressBarValue() {
        return this.progressBarDOM.value;
    }

    setProgressBarValue(value) {
        this.progressBarDOM.value = value;
    }

    getDomainInputValue() {
        return this.DOMDomain.value;
    }

    throwWarning(message) {
        this.setText(this.DOMValidateField, message);
        this.setVisible(this.DOMValidateField);
        this.setWarning(this.DOMInputSearchGroup);
    }

    showResult() {
        this.setVisible(this.whoisResultDOM);
    }

    fillDataDomain(domain) {
        domain.nsRecords.forEach((nr, i) => {
            var template = `<label for="dns-show" class="col-xs-2 col-form-label">DNS ${i + 1}</label>
                            <div class="col-xs-10">
                            <p class="form-control" id="dns-show">${nr}</p>
                            </div>`
            this.nsRecordsShow.innerHTML += template;
        })

        this.nameShow.innerText = domain.name;
        this.ipShow.innerText = domain.ip;
        this.hostShow.innerText = domain.host;
        this.whoisShow.innerText = domain.whois;

        progressBar.upProgressBar();
    }

    setInvisible(domItem) {
        domItem.classList.add('d-none');
    }

    setVisible(domItem) {
        domItem.classList.remove('d-none');
    }

    setText(domItem, text) {
        domItem.innerText = text;
        return domItem;
    }

    setWarning(domItem){
       domItem.classList.add('has-warning');
    }

    removeWarning(domItem) {
        domItem.classList.remove('has-warning');
    }

    removeDnsList() {
        $('#dns-form').children().remove();  
    }
}

class App {
    constructor() {
        this.messages = {
            invalidDomain : "Domínio inválido, digite um domínio existente...",
            errorServer: "Ocorreu algum erro ou o domínio não está registrado..."
        }
    }

    async startSearch(domainName) {
        this.resetApp();

        progressBar.startLoad();

        var domain = domainName == null ? await Domain.search(domManager.getDomainInputValue()) : await Domain.search(domainName);

        console.log(domain)

        if (!domain.success) return;

        progressBar.upProgressBar();

        domManager.fillDataDomain(domain);

        this.doneSearch();

        return domain;
    }

    doneSearch(resultDOM) {
        progressBar.stopLoad();
        domManager.showResult();
    }

    resetApp() {
        domManager.resetDOM();
    }

    formIsValid() {
        var isValid = Domain.isValid(domManager.getDomainInputValue());

        if (!isValid) {
            domManager.throwWarning(this.messages.invalidDomain);
   
        }

        return isValid;
    }
}

class Domain {
    constructor(response) {
        if (typeof response === 'undefined') {
            throw new Error('Cannot be called directly');
        }

        if (response && typeof response.request !== 'undefined') {
            this.name = response.request.name;
            this.ip = response.request.ip;
            this.host = response.request.hostedAt;
            this.whois = response.request.whoIs;
            this.nsRecords = response.request.nsRecords;
            this.success = response.success;
        }

        if (response && response.request === 'undefined') {
            
            this.success = response.success;
        }
    }

    static isValid(domainName) {
        const validDomainRegex = /^[a-zA-Z0-9][a-zA-Z0-9-]{1,61}[a-zA-Z0-9]\.(([a-zA-Z]{2,})|([a-zA-Z]{2,}\.[a-zA-Z]{2,2}))$/;

        return domainName.match(validDomainRegex);
    }

    static async search(domainName) {
        return await Domain.init(domainName);

    }

    static init(domainName) {
        return api.getDomain(domainName)
            .then(function (response) {
                return new Domain(response);
            })
    }
}

class Api {
  async request (method, url, body) {
    if (body) {
      body = JSON.stringify(body)
    }

    const request = new Request('/api/' + url, {
      method: method,
      body: body,
      credentials: 'same-origin',
      onUploadProgress: (p) => {
          console.log(p)
      },
      headers: new Headers({
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      })
    })

      const resp = await fetch(request);

      if (!resp.ok) {
          throw new this.DomainSearchExpection(resp.status);
       }

       const jsonResult = await resp.json()

       return jsonResult
    }

    DomainSearchExpection(status) {
        this.status = status;
        this.messages = app.messages;
    }

    async getDomain(domainOrIp) {
        try {
            var result = await this.request('GET', `domain/${domainOrIp}`);

            return { request: result, success: true } 
        } catch (e) {
            if (e.status === 400) {
                app.resetApp();
                domManager.throwWarning(e.messages.invalidDomain);
                return {success : false};
            }

            if (e.status === 500) {
                app.resetApp();
                domManager.throwWarning(e.messages.errorServer)
                return { success: false };
            }
        }
  }
}

const app = new App();
const api = new Api();
const domManager = new DOMManager();
const progressBar = new ProgressBar();

var callback = () => {
    const btnNewSearch = document.getElementById('btn-search');
    const btnSearched = $('.domain-searched-btn');

    if (btnNewSearch) {
      btnNewSearch.onclick = async () => {
          if (!app.formIsValid()) return;

          app.startSearch();       
        }

    if ($(btnSearched)) {
        $(btnSearched).on('click', (event) => {
            app.startSearch($(event.target).attr('data-domain'));
        })    
    }
  }
}

if (document.readyState === 'complete' || (document.readyState !== 'loading' && !document.documentElement.doScroll)) {
    callback()
    
} else {
  document.addEventListener('DOMContentLoaded', callback)
}

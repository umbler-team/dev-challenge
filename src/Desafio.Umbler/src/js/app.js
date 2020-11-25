const Request = window.Request
const Headers = window.Headers
const fetch = window.fetch

class ProgressBar {
    constructor() {
        this.totalSteps = 2;
        this.maximumPercentage = 100;
        this.progressBarStep = 100 / this.totalSteps;
        this.progressBarPercentage = 100 / this.totalSteps;
        this.progressBarDOM = document.getElementById('progress-request');
    }

    upProgressBar() {
        this.progressBarPercentage += this.progressBarStep;

        this.progressBarDOM.value = this.progressBarPercentage;
    }

    startLoad() {
        DOMManager.setVisible(this.progressBarDOM);

    }

    stopLoad() {
        DOMManager.setInvisible(this.progressBarDOM);

    }
}

class DOMManager {
    static fillDataDomain(domain) {
        const nameShow = document.getElementById("name-show");
        const ipShow = document.getElementById("ip-show");
        const hostShow = document.getElementById("host-show");
        const whoisShow = document.getElementById("whois-show");
        const nsRecordsShow = document.getElementById("dns-form");

        domain.nsRecords.forEach((nr, i) => {
            var template = `<label for="dns-show" class="col-xs-2 col-form-label">DNS ${i + 1}</label>
                            <div class="col-xs-10">
                            <p class="form-control" id="dns-show">${nr}</p>
                            </div>`
            nsRecordsShow.innerHTML += template;
        })

        nameShow.innerText = domain.name;
        ipShow.innerText = domain.ip;
        hostShow.innerText = domain.host;
        whoisShow.innerText = domain.whois;

        progressBar.upProgressBar();
    }

    static setInvisible(domItem) {
        domItem.classList.add('d-none');
    }

    static setVisible(domItem) {
        domItem.classList.remove('d-none');
    }

    static setText(domItem, text) {
        domItem.innerText = text;
        return domItem;
    }

    static setWarning(domItem, className){
       domItem.classList.add('has-warning');
    }

    static removeWarning(domItem, className) {
        domItem.classList.remove('has-warning');
    }

    static removeDnsList() {
        $('#dns-form').children().remove();  
    }
}

class App {
    constructor() {
        this.validDomainRegex = /^[a-zA-Z0-9][a-zA-Z0-9-]{1,61}[a-zA-Z0-9]\.(([a-zA-Z]{2,})|([a-zA-Z]{2,}\.[a-zA-Z]{2,2}))$/;
        this.DOMValidateField = document.getElementById('validate-field');
        this.DOMInputSearchGroup = document.getElementById('input-group-search');
        this.DOMDomain = document.getElementById('txt-search');
        this.whoisResultDOM = document.getElementById('whois-results');
        this.messages = {
            invalidDomain : "Domínio inválido, digite um domínio existente..."
        }
    }

    async startSearch(domainName) {
        this.resetApp();

        progressBar.startLoad(this.whoisResultDOM);

        var domain = domainName == null ? await Domain.search(this.DOMDomain.value) : await Domain.search(domainName);

        progressBar.upProgressBar();

        DOMManager.fillDataDomain(domain);

        this.finishSearch(this.whoisResultDOM);

        return await Domain.search(domainName);
    }

    finishSearch(resultDOM) {
        progressBar.stopLoad();
        DOMManager.setVisible(resultDOM);
    }

    resetApp() {
        DOMManager.removeWarning(this.DOMInputSearchGroup);
        DOMManager.setInvisible(this.DOMValidateField);
        DOMManager.setInvisible(this.whoisResultDOM);
        DOMManager.removeDnsList();
    }

    formIsValid() {
        if (!this.DOMDomain.value.match(this.validDomainRegex)) {
            DOMManager.setText(this.DOMValidateField, this.messages.invalidDomain);
            DOMManager.setVisible(this.DOMValidateField);
            DOMManager.setWarning(this.DOMInputSearchGroup);

            return false;
        }

        return true;
    }
}

class Domain {
    constructor(response) {
        if (typeof response === 'undefined') {
            throw new Error('Cannot be called directly');
        }

        if (response) {
            this.name = response.name;
            this.ip = response.ip;
            this.host = response.hostedAt;
            this.whois = response.whoIs;
            this.nsRecords = response.nsRecords;
        }
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

    const resp = await fetch(request)
    if (!resp.ok && resp.status !== 400) {
      throw Error(resp.statusText)
    }

    const jsonResult = await resp.json()

    if (resp.status === 400) {
      jsonResult.requestStatus = 400
    }

    return jsonResult
  }

  async getDomain (domainOrIp) {
    return this.request('GET', `domain/${domainOrIp}`)
  }
}

const api = new Api();
const progressBar = new ProgressBar();


var callback = () => {
    const btnNewSearch = document.getElementById('btn-search');
    const btnSearched = $('.domain-searched-btn');
    const app = new App();

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

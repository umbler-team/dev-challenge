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
        domItem.classList.add('invisible');
    }

    static setVisible(domItem) {
        domItem.classList.remove('invisible');
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

    static async startSearch(domainName) {
        return await Domain.init(domainName);
    }

    finishSearch(resultDOM) {
        progressBar.stopLoad();
        DOMManager.setVisible(resultDOM);
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
  const btn = document.getElementById('btn-search')
  const domainName = document.getElementById('txt-search')
  const whoisResultDOM = document.getElementById('whois-results');

  if (btn) {
      btn.onclick = async () => {
          DOMManager.setInvisible(whoisResultDOM);
          progressBar.startLoad(whoisResultDOM);

          var domain = await Domain.startSearch(domainName.value)

          progressBar.upProgressBar();

          DOMManager.fillDataDomain(domain);

          domain.finishSearch(whoisResultDOM);
    }
  }
}

if (document.readyState === 'complete' || (document.readyState !== 'loading' && !document.documentElement.doScroll)) {
  callback()
} else {
  document.addEventListener('DOMContentLoaded', callback)
}

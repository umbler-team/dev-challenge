const Request = window.Request
const Headers = window.Headers
const fetch = window.fetch

class Api {
  async request (method, url, body) {
    if (body) {
      body = JSON.stringify(body)
    }

    const domain = input.value.trim();

      if (!domain.includes(".")) {
      alert("Digite um domínio válido");
      return;
}
    const request = new Request('/api/' + url, {
      method: method,
      body: body,
      credentials: 'same-origin',
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

const api = new Api()

var callback = () => {
  const btn = document.getElementById('btn-search')
  const txt = document.getElementById('txt-search')
  const result = document.getElementById('whois-results')

  if (btn) {
    btn.onclick = async () => {
      const response = await api.getDomain(txt.value)
      if (response) {
        result.innerHTML = `
          <p><strong>Domínio:</strong> ${response.name}</p>
          <p><strong>IP:</strong> ${response.ip}</p>
          <p><strong>Hospedado em:</strong> ${response.hostedAt}</p>
`
      }
    }
  }
}

if (document.readyState === 'complete' || (document.readyState !== 'loading' && !document.documentElement.doScroll)) {
  callback()
} else {
  document.addEventListener('DOMContentLoaded', callback)
}

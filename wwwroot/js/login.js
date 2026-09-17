document.addEventListener('DOMContentLoaded', () => {
    document.querySelector('#btn-login').onclick = login;
});

async function login(event)
{
    let email = document.querySelector('#email').value;
    let password = document.querySelector('#password').value;

    var payload = {
        email: email,
        password: password
    };

    const response = fetch('/login/authenticate', {
      method: 'POST', // Specifies the request type
      headers: {
        'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    });

     if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
      error = await response.json();
      alert(error.message)
      throw new Error(`HTTP error! Status: ${response.status}`);      
    }

    const data = response.json(); 
    console.log(data);
    window.location = '/';

}
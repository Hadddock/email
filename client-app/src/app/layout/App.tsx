import React from 'react';
import './styles.css';
import {useState} from "react";

function App() {

  const [email, setEmail] = useState("");
  const [subject, setSubject] = useState("");
  const [body, setBody] = useState("");
  
  let handleSubmit = async (e:any) => {
    e.preventDefault();
    try {
      let x = JSON.stringify({
        recipientEmail: email,
        messageSubject: subject,
        messageBody: body,
      });

      var xhr = new XMLHttpRequest();
      xhr.open("POST", "http://localhost:5000/message", true);
      xhr.setRequestHeader('Content-Type', 'application/json');
      xhr.send(x);

    } catch (err) {
      console.log(err);
    }
  };

  return (
    <div className="App">
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          value={email}
          placeholder="Recipient Email Address"
          onChange={(e) => setEmail(e.target.value)}
        />
        <input
          type="text"
          value={subject}
          placeholder="Message Subject"
          onChange={(e) => setSubject(e.target.value)}
        />
        <input
          type="text"
          value={body}
          placeholder="Message Body"
          onChange={(e) => setBody(e.target.value)}
        />

        <button type="submit">Send</button>

        <div className="message">{<p>Please enter the message's recipient email address, subject, and body respectively. Then press Send.</p>}</div>
      </form>
    </div>
  );
}

export default App;

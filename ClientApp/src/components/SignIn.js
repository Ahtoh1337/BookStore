import { useContext, useState } from "react";
import { UserContext } from "../App";
import { useNavigate } from "react-router-dom";

function SignIn() {
  const [userData, setUserData] = useState({ email: "", password: "" });
  const [message, setMessage] = useState("");
  const navigate = useNavigate();
  const userContext = useContext(UserContext);

  async function sendLoginRequest(e) {
    e.preventDefault();
    console.log(`Sending request: ${JSON.stringify(userData)}`);

    const response = await fetch("login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(userData),
    });

    const body = await response.json();

    if (response.ok) {
      userContext.login(userData.email, body.accessToken, body.refreshToken);
      navigate("/");
    } else {
      setMessage(`Incorrect email or password. (${response.status})`);
    }
  }

  return (
    <div>
      <form onSubmit={sendLoginRequest}>
        <h1>Sign In</h1>
        <div>
          <label>
            Email:
            <input
              required
              onChange={(e) =>
                setUserData({ ...userData, email: e.currentTarget.value })
              }
            />
          </label><br />
          <label>
            Password:
            <input
              type="password"
              required
              onChange={(e) =>
                setUserData({ ...userData, password: e.currentTarget.value })
              }
            />
          </label>
        </div>
        <button type="submit">
          Sign In
        </button>
        <div>{message}</div>
      </form>
    </div>
  );
}

export default SignIn;

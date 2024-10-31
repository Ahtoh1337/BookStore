import { useContext, useState } from "react";
import { UserContext } from "../App";
import { useNavigate } from "react-router-dom";

export default function SignUp({ props }) {
  const [userData, setUserData] = useState({
    email: "",
    password: "",
    confirmPassword: "",
  });
  const [message, setMessage] = useState("");
  const navigate = useNavigate();
  const userContext = useContext(UserContext);

  async function sendRegisterRequest(e) {
    e.preventDefault();

    if (userData.password != userData.confirmPassword) {
      setMessage("Passwords don't match.");
      return;
    }

    const regData = {
      email: userData.email,
      password: userData.password,
    };

    const registerResponse = await fetch("register", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(regData),
    });

    console.log(`Registration: ${registerResponse.status}`);

    if (!registerResponse.ok) {
      const registerData = await registerResponse.json();
      setMessage(JSON.stringify(registerData.errors));
      return;
    }

    const loginResponse = await fetch("login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(regData),
    });

    console.log(`${loginResponse.status} ${loginResponse.statusText}`);

    const loginData = await loginResponse.json();

    if (loginResponse.ok) {
      userContext.login(userData.email, loginData.accessToken);
      navigate("/");
    } else {
      setMessage(`Incorrect email or password. (${loginResponse.status})`);
    }
  }

  return (
    <div>
      <form onSubmit={sendRegisterRequest}>
        <h1>Sign Up</h1>
        <label>
          Email:
          <input
            required
            onChange={(e) =>
              setUserData({ ...userData, email: e.currentTarget.value })
            }
          />
        </label>
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
        <label>
          Confirm Password:
          <input
            type="password"
            required
            onChange={(e) =>
              setUserData({
                ...userData,
                confirmPassword: e.currentTarget.value,
              })
            }
          />
        </label>
        <button type="submit">Sign Up</button>
      </form>
      <div>{message}</div>
    </div>
  );
}

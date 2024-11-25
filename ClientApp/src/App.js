import React, { Component, createContext, useEffect, useState } from "react";
import Cookies from "js-cookie";
import { Route, Routes, useNavigate } from "react-router-dom";
import AppRoutes from "./AppRoutes";
import { Layout } from "./components/Layout";
import "./custom.css";

export const UserContext = createContext(null);

export default function App({ props }) {
  const [currentUser, setCurrentUser] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    if (Cookies.get("accessToken") && Cookies.get("userName")) {
      setCurrentUser(Cookies.get("userName"));
    }
  }, []);

  function login(name, accessToken, refreshToken) {
    const expiration = new Date();
    expiration.setHours(expiration.getHours() + 1);
    Cookies.set("accessToken", accessToken, { expires: expiration });
    Cookies.set("refreshToken", refreshToken, { expires: expiration });
    Cookies.set("userName", name, { expires: expiration });
    const refresh = new Date(expiration.getTime());
    refresh.setMinutes(refresh.getMinutes() - 30);
    Cookies.set("refresh", refresh.getTime(), { expires: expiration });
    setCurrentUser(name);
  }

  function logout() {
    navigate("/");
    Cookies.remove("accessToken");
    Cookies.remove("refreshToken");
    Cookies.remove("userName");
    Cookies.remove("refresh");
    setCurrentUser(null);
  }

  async function authFetch(url, request = {}) {
    let accessToken = Cookies.get("accessToken");
    const refreshToken = Cookies.get("refreshToken");
    const refresh = Cookies.get("refresh");
    const userName = Cookies.get("userName");

    if (accessToken && refreshToken && refresh && userName) {
      let authHeader = "Bearer " + accessToken;

      if (new Date().getTime() > refresh) {
        const refreshResponse = await fetch("/refresh", {
          method: "POST",
          headers: {
            Authorization: authHeader,
            "Content-Type": "application/json",
          },
          body: JSON.stringify({refreshToken: refreshToken}),
        });

        const refreshBody = await refreshResponse.json();

        login(userName, refreshBody.accessToken, refreshBody.refreshToken);

        accessToken = Cookies.get("accessToken");
        authHeader = "Bearer " + accessToken;
      }

      if (request.headers) {
        request.headers = { ...request.headers, Authorization: authHeader };
      } else {
        request = {
          headers: {
            Authorization: authHeader,
          },
        };
      }
    }

    return fetch(url, request);
  }

  return (
    <UserContext.Provider value={{ currentUser, login, logout, authFetch }}>
      <Layout>
        <Routes>
          {AppRoutes.map((route, index) => {
            const { element, ...rest } = route;
            return <Route key={index} {...rest} element={element} />;
          })}
        </Routes>
      </Layout>
    </UserContext.Provider>
  );
}

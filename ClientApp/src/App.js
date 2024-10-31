import React, { Component, createContext, useEffect, useState } from "react";
import Cookies from "js-cookie";
import { Route, Routes } from "react-router-dom";
import AppRoutes from "./AppRoutes";
import { Layout } from "./components/Layout";
import "./custom.css";

export const UserContext = createContext(null);

export default function App({ props }) {
  const [currentUser, setCurrentUser] = useState(null);

  useEffect(() => {
    if (Cookies.get("accessToken") && Cookies.get("userName")) {
      setCurrentUser(Cookies.get("userName"));
    }
  });

  function login(name, accessToken) {
    const expiration = new Date();
    expiration.setHours(expiration.getHours() + 1);
    Cookies.set("accessToken", accessToken, { expires: expiration });
    Cookies.set("userName", name, { expires: expiration });
    setCurrentUser(name);
    console.log("Set accessToken which expires at: " + expiration);
  }

  function logout() {
    Cookies.remove("accessToken");
    Cookies.remove("userName");
    setCurrentUser(null);
  }

  return (
    <UserContext.Provider value={{currentUser, login, logout}}>
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

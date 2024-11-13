import React, { useContext, useState } from "react";
import {
  Collapse,
  Navbar,
  NavbarBrand,
  NavbarText,
  NavbarToggler,
  NavItem,
  NavLink,
} from "reactstrap";
import { Link } from "react-router-dom";
import "./NavMenu.css";
import { UserContext } from "../App";

export function NavMenu() {
  {
    const [collapsed, setCollapsed] = useState(true);
    const userContext = useContext(UserContext);

    return (
      <header>
        <Navbar
          className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3"
          container
          light
        >
          <NavbarBrand tag={Link} to="/">
            BookStore
          </NavbarBrand>
          {userContext.currentUser && (
            <NavbarText>Welcome, {userContext.currentUser}</NavbarText>
          )}
          <NavbarToggler
            onClick={() => setCollapsed(!collapsed)}
            className="mr-2"
          />
          <Collapse
            className="d-sm-inline-flex flex-sm-row-reverse"
            isOpen={collapsed}
            navbar
          >
            <ul className="navbar-nav flex-grow">
              <NavItem>
                <NavLink tag={Link} className="text-dark" to="/books">
                  Books
                </NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} className="text-dark" to="/authors">
                  Authors
                </NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} className="text-dark" to="/genres">
                  Genres
                </NavLink>
              </NavItem>
              <NavItem>
                {userContext.currentUser ? (
                  <NavLink
                    tag={Link}
                    className="text-dark fw-bold"
                    onClick={userContext.logout}
                  >
                    Sign Out
                  </NavLink>
                ) : (
                  <NavLink
                    tag={Link}
                    className="text-dark fw-bold"
                    to="/sign-in"
                  >
                    Sign In
                  </NavLink>
                )}
              </NavItem>
              {!userContext.currentUser && (
                <NavItem>
                  <NavLink
                    tag={Link}
                    className="text-dark fw-bold"
                    to="/sign-up"
                  >
                    Sign Up
                  </NavLink>
                </NavItem>
              )}
            </ul>
          </Collapse>
        </Navbar>
      </header>
    );
  }
}

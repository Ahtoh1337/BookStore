import Author from "./components/Author";
import AuthorList from "./components/AuthorList";
import Book from "./components/Book";
import BookList from "./components/BookList";
import Genre from "./components/Genre";
import GenreList from "./components/GenreList";
import { Home } from "./components/Home";
import SignIn from "./components/SignIn";
import SignUp from "./components/SignUp";

const AppRoutes = [
  {
    index: true,
    element: <Home />,
  },
  {
    path: "/sign-in",
    element: <SignIn />,
  },
  {
    path: "/sign-up",
    element: <SignUp />,
  },
  {
    path: "/genres",
    element: <GenreList />,
  },
  {
    path: "/genres/:genreId",
    element: <Genre />,
  },
  {
    path: "/books",
    element: <BookList />,
  },
  {
    path: "/books/:bookId",
    element: <Book />,
  },
  {
    path: "/authors",
    element: <AuthorList />,
  },
  {
    path: "/authors/:authorId",
    element: <Author />,
  },
];

export default AppRoutes;

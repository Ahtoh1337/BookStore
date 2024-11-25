import { useContext, useEffect, useState } from "react";
import { UserContext } from "../App";
import BookTable from "./BookTable";

export default function BookList() {
  const [data, setData] = useState({ books: [], state: "loading" });
  const userContext = useContext(UserContext);

  useEffect(() => {
    async function func() {
      const response = await userContext.authFetch("api/books");

      if (response.ok) {
        setData({ books: await response.json(), state: "ok" });
      } else {
        setData({ state: "fail" });
      }
    }

    func();
  }, []);

  return (
    <div>
      <h1>Books</h1>
      {data.state == "loading" && <p>Loading...</p>}
      {data.state == "fail" && <p>Loading failed.</p>}
      {data.state == "ok" && <BookTable books={data.books} />
      }
    </div>
  );
}

import { Link } from "react-router-dom";

function BookTable({ books, withRating = false, withPrice = true }) {
  if (books == null || books == undefined) throw "'books' prop required";
  return (
    <table className="table table-hover">
      <thead>
        <tr>
          <th>Title</th>
          {withPrice && <th>Price</th>}
          <th>Publisher</th>
          {withRating && <th>Rating</th>}
        </tr>
      </thead>
      <tbody>
        {books.map((b) => (
          <tr key={b.bookId}>
            <td>
              <Link to={`/books/${b.bookId}`}>{b.title}</Link>
            </td>
            {withPrice && <td>{b.price} UAH</td>}
            <td>
              "{b.publisher}" ({b.publishedYear})
            </td>
            {withRating && <td>{b.rating ?? "Not rated"}</td>}
          </tr>
        ))}
      </tbody>
    </table>
  );
}

export default BookTable;

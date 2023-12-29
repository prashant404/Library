
import { Component, OnInit } from '@angular/core';
import { BookService } from '../services/book.service';
import { AuthService } from '../services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { BookDetailModalComponent } from '../book-detail-modal/book-detail-modal.component';

@Component({
  selector: 'app-book-list',
  templateUrl: './book-list.component.html',
  styleUrls: ['./book-list.component.css']
})
export class BookListComponent implements OnInit {
  books: any[] = [];
  newBook: any = {};
  searchQuery: string = '';
  selectedBook: any;
  modalReference: any;

  constructor(
    private bookService: BookService,
    private authService: AuthService,
    private toastr: ToastrService,
    private modalService: NgbModal
  ) { }

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks(): void {
    this.bookService.getAllBooks().subscribe(
      (books: any[]) => {
        this.books = books;
      },
      (error) => {
        console.error('Error fetching books:', error);
      }
    );
  }

  borrowBook(bookId: number): void {
    this.authService.getUserTokensValue().subscribe(
      (tokens: number) => {
        if (tokens >= 1) {
          this.bookService.borrowBook(bookId).subscribe(
            (responseText: any) => {
              console.log(responseText);
              this.authService.updateTokensAfterBorrow();
              this.toastr.success('Book borrowed successfully!', 'Success');
            },
            (error) => {
              console.error('Error borrowing book:', error);
              this.toastr.error('Error borrowing book. Please try again.', 'Error');
            },
            () => {
              this.loadBooks();
            }
          );
        } else {
          console.error('Insufficient tokens for borrowing');
          this.toastr.warning('You have insufficient tokens for borrowing.', 'Warning');
        }
      },
      (error) => {
        console.error('Error fetching user tokens:', error);
        this.toastr.error('Error fetching user tokens. Please try again.', 'Error');
      }
    );
  }

  openAddBookModal(content: any): void {
    this.modalReference = this.modalService.open(content, { centered: true });
  }

  postBook(): void {
    console.log('New Book:', this.newBook);
    this.bookService.postBook(this.newBook).subscribe(
      (response: any) => {
        console.log(response);
        this.toastr.success('Book added successfully!', 'Success');
        this.modalReference.close();
        this.newBook = {};
        this.loadBooks();
      },
      (error) => {
        console.error('Error adding book:', error);
        this.toastr.error('Error adding book', 'Error');
      }
    );
  }

  searchBooks(): void {
    this.bookService.searchBooks(this.searchQuery).subscribe(
      (books: any[]) => {
        this.books = books;
      },
      (error) => {
        console.error('Error searching books:', error);
      }
    );
  }
  openBookDetailModal(book: any): void {
    const modalRef = this.modalService.open(BookDetailModalComponent, { centered: true });
    modalRef.componentInstance.book = book;
  }
}

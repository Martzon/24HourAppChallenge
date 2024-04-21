import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { User } from 'src/app/_shared/models/user.model';
import { UserService } from 'src/app/_shared/services/user.service';

@Component({
  selector: 'app-add-edit-user',
  templateUrl: './add-edit-user.component.html',
  styleUrls: ['./add-edit-user.component.css']
})
export class AddEditUserComponent {
  @Input() user!: User;
  @Input() action!: string;

  constructor(
    public modal: NgbActiveModal, 
    private userService: UserService
  ) {}

  saveUser() {
    if (this.action === 'Update') {
      this.userService.updateUser(this.user).subscribe({
        next: updatedUser => {
          console.log('User updated:', updatedUser);
        },
        error: err => {
          console.error('Failed to edit user:', err);
          // Handle error
        }
      });
    } else {
      const request = this.userRequest();
      this.userService.AddUser(request).subscribe({
        next: updatedUser => {
          console.log('User inserted:', updatedUser);
        },
        error: err => {
          console.error('Failed to add user:', err);
          // Handle error
        }
      });
    }
    this.modal.close();
  }

  userRequest() {
    const request: Record<string, unknown> = {
      email: this.user.email,
      firstName: this.user.firstName,
      lastName: this.user.lastName,
      isActive: true,
      password: "P@ssW0rd",
    };
    return request;
  }
}

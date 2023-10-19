import { ChatServiceService } from './../Services/chat-service.service';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  userForm:FormGroup=new FormGroup({});
  isSubmitted:boolean=false;
  apiErrorMessages:string[]=[];
  isOpenChat:boolean=false;

  constructor(private formBuilder:FormBuilder,private chatService:ChatServiceService) { }

  ngOnInit(): void {
    this.initializedForm();
  }

  initializedForm(){
    this.userForm=this.formBuilder.group({
      name:['',[Validators.required,Validators.minLength(3),Validators.maxLength(15)]]
    });
  }

  submitForm(){
    this.isSubmitted=true;
    this.apiErrorMessages=[];

    if (this.userForm.valid) {
      this.chatService.registerUser(this.userForm.value).subscribe({
        next:()=>{
          this.chatService.myName=this.userForm.get('name')?.value;
          this.isOpenChat=true;
          this.userForm.reset();   
          this.isSubmitted=false;       
        },
        error:error=>{
          if(typeof(error.error)!=='object'){
              this.apiErrorMessages.push(error.error);
          }
        }
      });    
    }

  }

  closeChat(){
    this.isOpenChat=false;
  }

}

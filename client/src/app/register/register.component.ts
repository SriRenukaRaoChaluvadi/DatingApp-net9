import { Component, inject, OnInit, output } from '@angular/core';
import {  AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { JsonPipe, NgIf } from '@angular/common';
import { TextInputComponent } from "../_forms/text-input/text-input.component";
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker'
import { DatePickerComponent } from '../_forms/date-picker/date-picker.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, JsonPipe, TextInputComponent, BsDatepickerModule, DatePickerComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  registerForm:FormGroup=new FormGroup({});
  private accountService=inject(AccountService);
  private formBuilder=inject(FormBuilder);
  maxDate=new Date();
model:any={}
validationErrors:string[]|undefined;
//@Input() usersFromHomeComponent:any;
//usersFromHomeComponent=input.required<any>();
 cancelRegister=output<boolean>();
 private toastr=inject(ToastrService);
 private router=inject(Router);
  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear()-18);
  }
  initializeForm(){
    this.registerForm=this.formBuilder.group({
      gender:['male'],
      username:['', Validators.required],
      knownAs:['', Validators.required],
      dateOfBirth:['', Validators.required],
      city:['', Validators.required],
      country:['', Validators.required],
      password:['',[Validators.required,Validators.minLength(4),Validators.maxLength(8)]],
      confirmPassword:['',[Validators.required,this.matchValues('password')]],
    });
    this.registerForm.controls['password'].valueChanges.subscribe(()=>{
      next:()=>this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }
matchValues(matchTo:string):ValidatorFn{
  return (control:AbstractControl)=>{
    return control.value===control.parent?.get(matchTo)?.value?null:{isMatching:true}
  }
}

  
register(){
  const dob=this.getDateOnly(this.registerForm.get('dateOfBirth')?.value);
  this.registerForm.patchValue({dateOfBirth:dob});
  this.accountService.register(this.registerForm.value).subscribe({
    next:_ =>this.router.navigateByUrl('/members'),
    error:error=>this.validationErrors=error
  });
  console.log(this.registerForm.value);
  
}
cancelled(){
  this.cancelRegister.emit(false);
  console.log("cancelled");
}
private getDateOnly(dob:string|undefined){
  if(!dob)return;
  return new Date(dob).toISOString().slice(0,10);
}
}

import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';

@Component({
  selector: 'app-enum-select',
  templateUrl: './enum-select.component.html',
  styleUrls: ['./enum-select.component.scss']
})
export class EnumSelectComponent implements OnInit {
  @Input() label = 'Select option';
  @Input() enumType!: object;
  @Input() selectedValue!: string;
  @Output() selectionChange = new EventEmitter<string>();

  options: string[] = [];

  ngOnInit(): void {
    this.options = Object.values(this.enumType).filter(v => typeof v === 'string') as string[];
  }

  onChange(value: string): void {
    this.selectionChange.emit(value);
  }
}

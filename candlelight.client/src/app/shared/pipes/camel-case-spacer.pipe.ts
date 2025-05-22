import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'camelCaseSpacer'
})
export class CamelCaseSpacerPipe implements PipeTransform {
  transform(value: string): string {
    if (!value) return '';
    return value.replace(/([a-z])([A-Z])/g, '$1 $2');
  }
}

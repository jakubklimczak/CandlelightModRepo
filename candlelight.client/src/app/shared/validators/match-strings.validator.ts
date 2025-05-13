import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function matchStrings(
  stringOne: string,
  stringTwo: string,
): ValidatorFn {
  return (formGroup: AbstractControl): ValidationErrors | null => {
    const firstString = formGroup.get(stringOne)?.value;
    const secondString = formGroup.get(stringTwo)?.value;

    if (firstString !== secondString) {
      return { stringsMismatch: true };
    }

    return null;
  };
}

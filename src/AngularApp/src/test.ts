import 'zone.js/testing';
import { getTestBed } from '@angular/core/testing';
import { BrowserTestingModule, platformBrowserTesting } from '@angular/platform-browser/testing';

const testBed = getTestBed();

testBed.initTestEnvironment(
  BrowserTestingModule,
  platformBrowserTesting(),
);

console.log('Angular testing environment initialized successfully');

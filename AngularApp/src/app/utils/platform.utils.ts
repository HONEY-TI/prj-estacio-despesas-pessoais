import { Capacitor } from '@capacitor/core';

export function isNativeMobile(): boolean {
  return Capacitor.isNativePlatform();
}

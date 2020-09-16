import { deflate, inflate } from "pako";

export function handleShareLink(onContents: (contents : string | null) => void) {
  try {
    const rawHash = window.location.hash.substr(1);
    if (!rawHash) {
      onContents(null);
    }

    history.replaceState(null, null, ' ');
    const hashContents = inflate(atob(rawHash), { to: 'string' });
    if (!hashContents) {
      onContents(null);
    }

    onContents(hashContents);
  } catch {
    onContents(null);
  }
}

export function copyShareLinkToClipboard(content: string) {
  document.addEventListener('copy', function onCopy(e: ClipboardEvent) {
    const contentHash = btoa(deflate(content, { to: 'string' }));
    e.clipboardData.setData('text/plain', `https://aka.ms/bicepdemo#${contentHash}`);
    e.preventDefault();
    document.removeEventListener('copy', onCopy, true);
  });

  document.execCommand('copy');
}
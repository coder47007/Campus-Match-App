// Premium color palette for CampusMatch
// Modern dating app aesthetic with Abyssal Aqua theme

export const Colors = {
  // Primary gradient colors (Magenta / Deep Purple theme)
  primary: {
    gradient: ['#FF4081', '#D500F9', '#7C3AED'] as const,
    main: '#FF4081', // Neon Pink
    light: '#FF80AB',
    dark: '#C51162',
  },

  // Secondary accent colors
  secondary: {
    gradient: ['#E1BEE7', '#4A148C'] as const,
    main: '#E1BEE7', // Pale Purple
    light: '#F3E5F5',
    dark: '#4A148C', // Dark Purple
  },

  // Action colors
  like: '#00E676',           // Neon Green
  superLike: '#2979FF',      // Azure Blue
  dislike: '#FF1744',        // Neon Red
  rewind: '#FFCA28',         // Amber
  boost: '#E040FB',          // Purple Boost

  // Match celebration
  match: {
    gradient: ['#FF4081', '#7C3AED', '#660c6c'] as const,
  },

  // Enforce Deep Purple theme for Light mode as well
  light: {
    background: '#660c6c',     // Deep Magenta (Requested)
    surface: '#1a1a2e',        // Dark Blue/Purple (from Login Page)
    card: '#2A0E35',           // Darker Magenta Card
    text: '#FFFFFF',
    textSecondary: '#E1BEE7',  // Pale Purple
    textMuted: '#9C27B0',      // Purple Muted
    border: '#4A148C',         // Dark Purple
    icon: '#E1BEE7',
  },

  // Dark theme - Deep Purple
  dark: {
    background: '#660c6c',     // Deep Magenta (Requested)
    surface: '#1a1a2e',        // Dark Blue/Purple (from Login Page)
    card: '#2A0E35',           // Darker Magenta Card
    text: '#FFFFFF',
    textSecondary: '#E1BEE7',
    textMuted: '#9C27B0',
    border: '#4A148C',
    icon: '#E1BEE7',
  },

  // Status colors
  online: '#00E676',
  offline: '#9C27B0',
  typing: '#FF4081',

  // Overlay colors
  overlay: {
    light: 'rgba(26, 26, 46, 0.5)',
    dark: 'rgba(26, 26, 46, 0.85)',
  },

  // Glassmorphism
  glass: {
    background: 'rgba(102, 12, 108, 0.7)', // #660c6c glass
    border: 'rgba(225, 190, 231, 0.3)',    // Pale purple border
  },

  // Chat bubbles
  chat: {
    sent: '#FF4081',            // Pink
    received: '#2A0E35',        // Dark card color
    sentText: '#FFFFFF',
    receivedText: '#FFFFFF',
  },

  // Common
  white: '#FFFFFF',
  black: '#000000',
  transparent: 'transparent',
  error: '#FF1744',
  success: '#00E676',
  warning: '#FFCA28',
  info: '#2979FF',
};

export default Colors;

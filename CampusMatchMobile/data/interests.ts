export interface InterestCategory {
    id: string;
    title: string;
    items: InterestItem[];
}

export interface InterestItem {
    id: string;
    label: string;
    emoji: string;
}

export const interestCategories: InterestCategory[] = [
    {
        id: 'sports',
        title: 'Sports & Active',
        items: [
            { id: 'varsity', label: 'Varsity Athlete', emoji: '🏆' },
            { id: 'gym', label: 'Gym Rat', emoji: '🏋️' },
            { id: 'swimmer', label: 'Swimmer', emoji: '🏊‍♂️' },
            { id: 'hiker', label: 'Hiker', emoji: '🥾' },
            { id: 'runner', label: 'Runner', emoji: '🏃‍♀️' },
            { id: 'basketball', label: 'Basketball', emoji: '🏀' },
            { id: 'soccer', label: 'Soccer / Football', emoji: '⚽' },
            { id: 'volleyball', label: 'Volleyball', emoji: '🏐' },
            { id: 'climbing', label: 'Rock Climbing', emoji: '🧗' },
            { id: 'yoga', label: 'Yoga / Pilates', emoji: '🧘‍♀️' },
            { id: 'surfing', label: 'Surfing', emoji: '🏄' },
            { id: 'snowboarding', label: 'Snowboarding', emoji: '🏂' },
            { id: 'martial_arts', label: 'Martial Arts', emoji: '🥋' },
            { id: 'cycling', label: 'Cycling', emoji: '🚴' },
            { id: 'skateboarding', label: 'Skateboarding', emoji: '🛹' },
        ]
    },
    {
        id: 'creative',
        title: 'Creative & Arts',
        items: [
            { id: 'photography', label: 'Photography', emoji: '📸' },
            { id: 'musician', label: 'Musician', emoji: '🎸' },
            { id: 'digital_art', label: 'Digital Art', emoji: '🎨' },
            { id: 'writer', label: 'Writer / Poet', emoji: '✍️' },
            { id: 'filmmaking', label: 'Filmmaking', emoji: '🎥' },
            { id: 'fashion', label: 'Fashion Design', emoji: '👗' },
            { id: 'dancer', label: 'Dancer', emoji: '💃' },
            { id: 'makeup', label: 'Makeup Artist', emoji: '💄' },
            { id: 'pottery', label: 'Pottery / Ceramics', emoji: '🏺' },
            { id: 'singing', label: 'Singing', emoji: '🎤' },
        ]
    },
    {
        id: 'intellectual',
        title: 'Intellectual & Academic',
        items: [
            { id: 'bookworm', label: 'Bookworm', emoji: '📚' },
            { id: 'coding', label: 'Tech / Coding', emoji: '💻' },
            { id: 'chess', label: 'Chess Player', emoji: '♟️' },
            { id: 'history', label: 'History Buff', emoji: '🏛️' },
            { id: 'science', label: 'Science Nerd', emoji: '🧪' },
            { id: 'entrepreneur', label: 'Entrepreneur', emoji: '💼' },
            { id: 'debater', label: 'Debater', emoji: '🗣️' },
            { id: 'anime', label: 'Anime / Manga', emoji: '⛩️' },
            { id: 'gamer', label: 'Gamer', emoji: '🎮' },
            { id: 'board_games', label: 'Board Games', emoji: '🎲' },
        ]
    },
    {
        id: 'social',
        title: 'Social & Campus Life',
        items: [
            { id: 'party', label: 'Party Animal', emoji: '🥂' },
            { id: 'greek', label: 'Greek Life', emoji: '🏛' },
            { id: 'volunteer', label: 'Volunteer', emoji: '🤝' },
            { id: 'foodie', label: 'Foodie', emoji: '🍕' },
            { id: 'coffee', label: 'Coffee Addict', emoji: '☕' },
            { id: 'clubbing', label: 'Clubbing', emoji: '🪩' },
            { id: 'concerts', label: 'Concert Goer', emoji: '🎫' },
            { id: 'traveler', label: 'Traveler', emoji: '✈️' },
            { id: 'thrifting', label: 'Thrift Shopping', emoji: '🛍️' },
            { id: 'plants', label: 'Plant Parent', emoji: '🪴' },
        ]
    },
    {
        id: 'niche',
        title: 'Niche & Chill',
        items: [
            { id: 'astrology', label: 'Astrologer', emoji: '🔮' },
            { id: 'cars', label: 'Car Enthusiast', emoji: '🏎️' },
            { id: 'cooking', label: 'Cooking', emoji: '🍳' },
            { id: 'meditation', label: 'Meditation', emoji: '🧘' },
            { id: 'dogs', label: 'Dog Lover', emoji: '🐶' },
        ]
    },
];

export const allInterests = interestCategories.flatMap(cat => cat.items);

export function searchInterests(query: string): InterestItem[] {
    if (!query) return [];
    const lowerQuery = query.toLowerCase();
    return allInterests.filter(item =>
        item.label.toLowerCase().includes(lowerQuery)
    );
}

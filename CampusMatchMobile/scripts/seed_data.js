
const { createClient } = require('@supabase/supabase-js');
const fs = require('fs');
const path = require('path');

// Configuration
const SUPABASE_URL = 'https://urtynghfmszehjwvligg.supabase.co';
const SUPABASE_ANON_KEY = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVydHluZ2hmbXN6ZWhqd3ZsaWdnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Njg0MDk2MDEsImV4cCI6MjA4Mzk4NTYwMX0.nLQd8gmmRH76HaYZ4kkOTbwTnm8EiOtdJ6GF9aHuEuA';

// Create a client with NO persistent session to start fresh
const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY, {
    auth: {
        persistSession: false,
        autoRefreshToken: false,
        detectSessionInUrl: false
    }
});

// Images to upload (Absolute paths)
const IMAGES = [
    { name: 'female_student_1', path: 'C:\\Users\\Shams\\.gemini\\antigravity\\brain\\23f93232-e31f-4790-98dc-0689ac8873c1\\female_student_1_1768778556120.png', gender: 'Female' },
    { name: 'female_student_2', path: 'C:\\Users\\Shams\\.gemini\\antigravity\\brain\\23f93232-e31f-4790-98dc-0689ac8873c1\\female_student_2_1768778582002.png', gender: 'Female' },
    { name: 'female_student_3', path: 'C:\\Users\\Shams\\.gemini\\antigravity\\brain\\23f93232-e31f-4790-98dc-0689ac8873c1\\female_student_3_1768778620702.png', gender: 'Female' },
    { name: 'male_student_1', path: 'C:\\Users\\Shams\\.gemini\\antigravity\\brain\\23f93232-e31f-4790-98dc-0689ac8873c1\\male_student_1_1768778543032.png', gender: 'Male' },
    { name: 'male_student_2', path: 'C:\\Users\\Shams\\.gemini\\antigravity\\brain\\23f93232-e31f-4790-98dc-0689ac8873c1\\male_student_2_1768778569343.png', gender: 'Male' },
    { name: 'male_student_3', path: 'C:\\Users\\Shams\\.gemini\\antigravity\\brain\\23f93232-e31f-4790-98dc-0689ac8873c1\\male_student_3_1768778595591.png', gender: 'Male' },
    { name: 'male_student_4', path: 'C:\\Users\\Shams\\.gemini\\antigravity\\brain\\23f93232-e31f-4790-98dc-0689ac8873c1\\male_student_4_1768778634330.png', gender: 'Male' },
];

const mockData = {
    female_student_1: { name: 'Sophia Miller', age: 20, major: 'Psychology', bio: 'Coffee addict & future therapist ☕️', uni: 'CampusMatch U' },
    female_student_2: { name: 'Emma Wilson', age: 19, major: 'Biology', bio: 'Nature lover 🌿 Hiking buddy needed!', uni: 'State College' },
    female_student_3: { name: 'Olivia Davis', age: 21, major: 'Fine Arts', bio: 'Always sketching. Let’s go to a museum! 🎨', uni: 'Arts Academy' },
    male_student_1: { name: 'Liam Johnson', age: 20, major: 'Computer Science', bio: 'Tech enthusiast & gamer 🎮', uni: 'Tech Inst' },
    male_student_2: { name: 'Noah Brown', age: 22, major: 'Business', bio: 'Crypto & chill 📈', uni: 'Business School' },
    male_student_3: { name: 'William Jones', age: 21, major: 'English Lit', bio: 'Bookworm looking for a study date 📚', uni: 'CampusMatch U' },
    male_student_4: { name: 'James Garcia', age: 19, major: 'Sports Management', bio: 'Always at the gym or the field 🏈', uni: 'State College' },
};

async function seedData() {
    console.log('Starting Supabase Seeding with Registration Flow...');

    // 1. Upload Images First (Using Anon Key - Public Bucket)
    // Uploads usually work with Anon key if bucket is Public RLS allowed.
    const imageMap = {}; // name -> publicUrl

    for (const img of IMAGES) {
        console.log(`Uploading ${img.name}...`);
        try {
            const fileContent = fs.readFileSync(img.path);
            const fileName = `${Date.now()}_${path.basename(img.path)}`;

            const { data: uploadData, error: uploadError } = await supabase.storage
                .from('avatars')
                .upload(fileName, fileContent, {
                    contentType: 'image/png',
                    upsert: true
                });

            if (uploadError) {
                console.error(`Upload failed for ${img.name}:`, uploadError.message);
                // Fallback
                imageMap[img.name] = img.gender === 'Female'
                    ? 'https://randomuser.me/api/portraits/women/44.jpg'
                    : 'https://randomuser.me/api/portraits/men/32.jpg';
            } else {
                const { data } = supabase.storage.from('avatars').getPublicUrl(fileName);
                imageMap[img.name] = data.publicUrl;
                console.log(`Uploaded: ${data.publicUrl}`);
            }
        } catch (err) {
            console.error(`File error for ${img.name}:`, err.message);
            imageMap[img.name] = img.gender === 'Female'
                ? 'https://randomuser.me/api/portraits/women/44.jpg'
                : 'https://randomuser.me/api/portraits/men/32.jpg';
        }
    }

    // 2. Register Users & Insert Profiles
    const createdStudents = [];

    for (const img of IMAGES) {
        const details = mockData[img.name];
        // Unique email for this run
        const randomStr = Math.floor(Math.random() * 9000) + 1000;
        const email = `${details.name.split(' ')[0].toLowerCase()}${randomStr}@campusmatch.test`;
        const password = 'TestPassword123!';

        console.log(`Registering ${details.name} (${email})...`);

        // Sign Up
        const { data: authData, error: authError } = await supabase.auth.signUp({
            email,
            password,
        });

        if (authError) {
            console.error(`Registration failed for ${details.name}:`, authError.message);
            continue;
        }

        if (!authData.user) {
            console.error(`No user returned for ${details.name}`);
            continue;
        }

        console.log(`User created (UUID: ${authData.user.id}). Inserting profile...`);

        // Wait a small bit for any auth triggers (if any)
        await new Promise(r => setTimeout(r, 500));

        // Insert Profile (Authenticated as the new user)
        // Note: Supabase JS client updates its session on signUp automatically.
        const { data: student, error: insertError } = await supabase
            .from('Students')
            .insert({
                // Id: authData.user.id, // Wait, Students.Id is SERIAL (Integer)? Or UUID?
                // IMPORTANT: The app assumes Students.Id is INTEGER.
                // We should NOT provide Id if it is Serial.
                // But wait, how do we link it to Auth User?
                // The app uses Email to link them!
                // So we just insert the email.
                Email: email,
                Name: details.name,
                Age: details.age,
                Major: details.major,
                Bio: details.bio,
                Gender: img.gender,
                University: details.uni,
                Year: 'Junior',
                PhotoUrl: imageMap[img.name],
                Photos: [imageMap[img.name]],
                CreatedAt: new Date().toISOString()
            })
            .select()
            .single();

        if (insertError) {
            console.error(`Profile insert failed for ${details.name}:`, insertError.message);
        } else {
            console.log(`SUCCESS: Created Student ${student.Name} (ID: ${student.Id})`);
            createdStudents.push(student);
        }

        // Sign out to prepare for next user
        await supabase.auth.signOut();
    }

    console.log(`\nDONE. Created ${createdStudents.length} students.`);
}

seedData();

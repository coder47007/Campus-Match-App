// University data for US and Canada
// Source: Curated from Hipo university-domains-list

export interface University {
    name: string;
    country: string;
    domain?: string;
}

// Top US and Canadian universities for autocomplete
export const universities: University[] = [
    // CANADA - Ontario
    { name: "University of Toronto", country: "Canada", domain: "utoronto.ca" },
    { name: "Western University", country: "Canada", domain: "uwo.ca" },
    { name: "Queen's University", country: "Canada", domain: "queensu.ca" },
    { name: "McMaster University", country: "Canada", domain: "mcmaster.ca" },
    { name: "University of Ottawa", country: "Canada", domain: "uottawa.ca" },
    { name: "York University", country: "Canada", domain: "yorku.ca" },
    { name: "University of Waterloo", country: "Canada", domain: "uwaterloo.ca" },
    { name: "Carleton University", country: "Canada", domain: "carleton.ca" },
    { name: "Toronto Metropolitan University", country: "Canada", domain: "torontomu.ca" },
    { name: "University of Guelph", country: "Canada", domain: "uoguelph.ca" },
    { name: "Wilfrid Laurier University", country: "Canada", domain: "wlu.ca" },
    { name: "Brock University", country: "Canada", domain: "brocku.ca" },
    { name: "Lakehead University", country: "Canada", domain: "lakeheadu.ca" },
    { name: "Trent University", country: "Canada", domain: "trentu.ca" },
    { name: "OCAD University", country: "Canada", domain: "ocadu.ca" },
    { name: "Ontario Tech University", country: "Canada", domain: "ontariotechu.ca" },
    { name: "Nipissing University", country: "Canada", domain: "nipissingu.ca" },
    { name: "Algoma University", country: "Canada", domain: "algomau.ca" },

    // CANADA - Ontario Colleges
    { name: "Seneca College", country: "Canada", domain: "senecacollege.ca" },
    { name: "Humber College", country: "Canada", domain: "humber.ca" },
    { name: "George Brown College", country: "Canada", domain: "georgebrown.ca" },
    { name: "Sheridan College", country: "Canada", domain: "sheridancollege.ca" },
    { name: "Conestoga College", country: "Canada", domain: "conestogac.on.ca" },
    { name: "Algonquin College", country: "Canada", domain: "algonquincollege.com" },
    { name: "Centennial College", country: "Canada", domain: "centennialcollege.ca" },
    { name: "Mohawk College", country: "Canada", domain: "mohawkcollege.ca" },
    { name: "Fanshawe College", country: "Canada", domain: "fanshawec.ca" },
    { name: "Durham College", country: "Canada", domain: "durhamcollege.ca" },
    { name: "Georgian College", country: "Canada", domain: "georgiancollege.ca" },
    { name: "St. Clair College", country: "Canada", domain: "stclaircollege.ca" },
    { name: "Lambton College", country: "Canada", domain: "lambtoncollege.ca" },

    // CANADA - Quebec
    { name: "McGill University", country: "Canada", domain: "mcgill.ca" },
    { name: "Université de Montréal", country: "Canada", domain: "umontreal.ca" },
    { name: "Concordia University", country: "Canada", domain: "concordia.ca" },
    { name: "Université Laval", country: "Canada", domain: "ulaval.ca" },
    { name: "Université de Sherbrooke", country: "Canada", domain: "usherbrooke.ca" },
    { name: "UQAM", country: "Canada", domain: "uqam.ca" },
    { name: "HEC Montréal", country: "Canada", domain: "hec.ca" },
    { name: "Polytechnique Montréal", country: "Canada", domain: "polymtl.ca" },

    // CANADA - British Columbia
    { name: "University of British Columbia", country: "Canada", domain: "ubc.ca" },
    { name: "Simon Fraser University", country: "Canada", domain: "sfu.ca" },
    { name: "University of Victoria", country: "Canada", domain: "uvic.ca" },
    { name: "Emily Carr University", country: "Canada", domain: "ecuad.ca" },
    { name: "Thompson Rivers University", country: "Canada", domain: "tru.ca" },
    { name: "Royal Roads University", country: "Canada", domain: "royalroads.ca" },
    { name: "Kwantlen Polytechnic University", country: "Canada", domain: "kpu.ca" },
    { name: "Capilano University", country: "Canada", domain: "capilanou.ca" },
    { name: "BCIT", country: "Canada", domain: "bcit.ca" },

    // CANADA - BC Colleges
    { name: "Langara College", country: "Canada", domain: "langara.ca" },
    { name: "Douglas College", country: "Canada", domain: "douglascollege.ca" },
    { name: "Vancouver Community College", country: "Canada", domain: "vcc.ca" },
    { name: "Camosun College", country: "Canada", domain: "camosun.ca" },
    { name: "Okanagan College", country: "Canada", domain: "okanagan.bc.ca" },
    { name: "College of New Caledonia", country: "Canada", domain: "cnc.bc.ca" },

    // CANADA - Alberta
    { name: "University of Alberta", country: "Canada", domain: "ualberta.ca" },
    { name: "University of Calgary", country: "Canada", domain: "ucalgary.ca" },
    { name: "Mount Royal University", country: "Canada", domain: "mtroyal.ca" },
    { name: "MacEwan University", country: "Canada", domain: "macewan.ca" },
    { name: "Athabasca University", country: "Canada", domain: "athabascau.ca" },
    { name: "University of Lethbridge", country: "Canada", domain: "uleth.ca" },
    { name: "NAIT", country: "Canada", domain: "nait.ca" },
    { name: "SAIT", country: "Canada", domain: "sait.ca" },
    { name: "Bow Valley College", country: "Canada", domain: "bowvalleycollege.ca" },
    { name: "NorQuest College", country: "Canada", domain: "norquest.ca" },
    { name: "Red Deer Polytechnic", country: "Canada", domain: "rdpolytech.ca" },

    // CANADA - Other Colleges
    { name: "Red River College Polytechnic", country: "Canada", domain: "rrc.ca" },
    { name: "Saskatchewan Polytechnic", country: "Canada", domain: "saskpolytech.ca" },
    { name: "Holland College", country: "Canada", domain: "hollandcollege.com" },
    { name: "College of the North Atlantic", country: "Canada", domain: "cna.nl.ca" },
    { name: "NSCC", country: "Canada", domain: "nscc.ca" },
    { name: "NBCC", country: "Canada", domain: "nbcc.ca" },

    // CANADA - Other Provinces
    { name: "University of Manitoba", country: "Canada", domain: "umanitoba.ca" },
    { name: "University of Winnipeg", country: "Canada", domain: "uwinnipeg.ca" },
    { name: "University of Saskatchewan", country: "Canada", domain: "usask.ca" },
    { name: "University of Regina", country: "Canada", domain: "uregina.ca" },
    { name: "Dalhousie University", country: "Canada", domain: "dal.ca" },
    { name: "Saint Mary's University", country: "Canada", domain: "smu.ca" },
    { name: "Acadia University", country: "Canada", domain: "acadiau.ca" },
    { name: "University of New Brunswick", country: "Canada", domain: "unb.ca" },
    { name: "Mount Allison University", country: "Canada", domain: "mta.ca" },
    { name: "Memorial University of Newfoundland", country: "Canada", domain: "mun.ca" },
    { name: "University of Prince Edward Island", country: "Canada", domain: "upei.ca" },

    // USA - Ivy League
    { name: "Harvard University", country: "United States", domain: "harvard.edu" },
    { name: "Yale University", country: "United States", domain: "yale.edu" },
    { name: "Princeton University", country: "United States", domain: "princeton.edu" },
    { name: "Columbia University", country: "United States", domain: "columbia.edu" },
    { name: "University of Pennsylvania", country: "United States", domain: "upenn.edu" },
    { name: "Brown University", country: "United States", domain: "brown.edu" },
    { name: "Cornell University", country: "United States", domain: "cornell.edu" },
    { name: "Dartmouth College", country: "United States", domain: "dartmouth.edu" },

    // USA - Top Universities
    { name: "Stanford University", country: "United States", domain: "stanford.edu" },
    { name: "MIT", country: "United States", domain: "mit.edu" },
    { name: "Caltech", country: "United States", domain: "caltech.edu" },
    { name: "Duke University", country: "United States", domain: "duke.edu" },
    { name: "Northwestern University", country: "United States", domain: "northwestern.edu" },
    { name: "University of Chicago", country: "United States", domain: "uchicago.edu" },
    { name: "Johns Hopkins University", country: "United States", domain: "jhu.edu" },
    { name: "Vanderbilt University", country: "United States", domain: "vanderbilt.edu" },
    { name: "Rice University", country: "United States", domain: "rice.edu" },
    { name: "Notre Dame University", country: "United States", domain: "nd.edu" },
    { name: "Georgetown University", country: "United States", domain: "georgetown.edu" },
    { name: "Emory University", country: "United States", domain: "emory.edu" },

    // USA - UCs
    { name: "UC Berkeley", country: "United States", domain: "berkeley.edu" },
    { name: "UCLA", country: "United States", domain: "ucla.edu" },
    { name: "UC San Diego", country: "United States", domain: "ucsd.edu" },
    { name: "UC Santa Barbara", country: "United States", domain: "ucsb.edu" },
    { name: "UC Davis", country: "United States", domain: "ucdavis.edu" },
    { name: "UC Irvine", country: "United States", domain: "uci.edu" },
    { name: "UC Santa Cruz", country: "United States", domain: "ucsc.edu" },
    { name: "UC Riverside", country: "United States", domain: "ucr.edu" },
    { name: "UC Merced", country: "United States", domain: "ucmerced.edu" },

    // USA - State Schools
    { name: "University of Michigan", country: "United States", domain: "umich.edu" },
    { name: "University of Virginia", country: "United States", domain: "virginia.edu" },
    { name: "UNC Chapel Hill", country: "United States", domain: "unc.edu" },
    { name: "University of Florida", country: "United States", domain: "ufl.edu" },
    { name: "University of Texas Austin", country: "United States", domain: "utexas.edu" },
    { name: "Georgia Tech", country: "United States", domain: "gatech.edu" },
    { name: "University of Wisconsin Madison", country: "United States", domain: "wisc.edu" },
    { name: "Ohio State University", country: "United States", domain: "osu.edu" },
    { name: "Penn State University", country: "United States", domain: "psu.edu" },
    { name: "University of Illinois", country: "United States", domain: "illinois.edu" },
    { name: "University of Washington", country: "United States", domain: "washington.edu" },
    { name: "University of Maryland", country: "United States", domain: "umd.edu" },
    { name: "Purdue University", country: "United States", domain: "purdue.edu" },
    { name: "Indiana University", country: "United States", domain: "indiana.edu" },
    { name: "University of Minnesota", country: "United States", domain: "umn.edu" },
    { name: "University of Colorado Boulder", country: "United States", domain: "colorado.edu" },
    { name: "University of Arizona", country: "United States", domain: "arizona.edu" },
    { name: "Arizona State University", country: "United States", domain: "asu.edu" },
    { name: "Michigan State University", country: "United States", domain: "msu.edu" },
    { name: "University of Iowa", country: "United States", domain: "uiowa.edu" },
    { name: "University of Oregon", country: "United States", domain: "uoregon.edu" },
    { name: "Oregon State University", country: "United States", domain: "oregonstate.edu" },

    // USA - Private Schools
    { name: "USC", country: "United States", domain: "usc.edu" },
    { name: "NYU", country: "United States", domain: "nyu.edu" },
    { name: "Boston University", country: "United States", domain: "bu.edu" },
    { name: "Boston College", country: "United States", domain: "bc.edu" },
    { name: "Northeastern University", country: "United States", domain: "northeastern.edu" },
    { name: "Carnegie Mellon University", country: "United States", domain: "cmu.edu" },
    { name: "Tufts University", country: "United States", domain: "tufts.edu" },
    { name: "Wake Forest University", country: "United States", domain: "wfu.edu" },
    { name: "Tulane University", country: "United States", domain: "tulane.edu" },
    { name: "Miami University", country: "United States", domain: "miamioh.edu" },
    { name: "University of Miami", country: "United States", domain: "miami.edu" },
    { name: "George Washington University", country: "United States", domain: "gwu.edu" },
    { name: "American University", country: "United States", domain: "american.edu" },
    { name: "Syracuse University", country: "United States", domain: "syr.edu" },
    { name: "Villanova University", country: "United States", domain: "villanova.edu" },
    { name: "Santa Clara University", country: "United States", domain: "scu.edu" },
    { name: "Pepperdine University", country: "United States", domain: "pepperdine.edu" },
    { name: "Loyola Marymount University", country: "United States", domain: "lmu.edu" },
    { name: "Fordham University", country: "United States", domain: "fordham.edu" },
    { name: "Drexel University", country: "United States", domain: "drexel.edu" },
    { name: "Temple University", country: "United States", domain: "temple.edu" },

    // USA - Tech Schools
    { name: "RIT", country: "United States", domain: "rit.edu" },
    { name: "Stevens Institute of Technology", country: "United States", domain: "stevens.edu" },
    { name: "Worcester Polytechnic Institute", country: "United States", domain: "wpi.edu" },
    { name: "Rensselaer Polytechnic Institute", country: "United States", domain: "rpi.edu" },
    { name: "Illinois Institute of Technology", country: "United States", domain: "iit.edu" },

    // USA - Art/Design Schools
    { name: "Parsons School of Design", country: "United States", domain: "newschool.edu" },
    { name: "Rhode Island School of Design", country: "United States", domain: "risd.edu" },
    { name: "Pratt Institute", country: "United States", domain: "pratt.edu" },
    { name: "School of Visual Arts", country: "United States", domain: "sva.edu" },
    { name: "California College of the Arts", country: "United States", domain: "cca.edu" },
    { name: "ArtCenter College of Design", country: "United States", domain: "artcenter.edu" },
];

// Function to search universities
export function searchUniversities(query: string, limit: number = 10): University[] {
    if (!query || query.length < 2) return [];

    const lowerQuery = query.toLowerCase();

    return universities
        .filter(uni =>
            uni.name.toLowerCase().includes(lowerQuery) ||
            (uni.domain && uni.domain.toLowerCase().includes(lowerQuery))
        )
        .slice(0, limit);
}

// Function to get university from email domain
export function getUniversityFromDomain(domain: string): University | undefined {
    const lowerDomain = domain.toLowerCase();
    return universities.find(uni => uni.domain === lowerDomain);
}

// Extract domain from email
export function extractDomainFromEmail(email: string): string | null {
    const match = email.match(/@(.+)$/);
    return match ? match[1].toLowerCase() : null;
}
